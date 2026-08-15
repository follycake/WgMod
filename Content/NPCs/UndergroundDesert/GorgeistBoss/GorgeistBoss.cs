using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.GlobalNPCs;
using WgMod.Content.Items.Armor.Vanity;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Pets;
using WgMod.Content.Items.Placeable.Furniture;
using WgMod.Content.Items.Weapons.Melee;
using WgMod.Content.Projectiles.Enemy.Gorgeist;

namespace WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;

[AutoloadBossHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class GorgeistBossBody : ModNPC
{
	public enum Flags
	{
		None = 0,
		SecondPhase = 1 << 0,
		DidAttack = 1 << 1
	}

	public enum State
	{
		None = 0,

		// Phase 1
		Idle,
		TossPlate,
		TossFood,
		CirclingPlayer,

		// Phase 2
		Enraged
	}

	public Flags BossFlags
	{
		get => (Flags)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	public State CurrentState
	{
		get => (State)(int)NPC.ai[1];
		set => NPC.ai[1] = (int)value;
	}

	public Vector2 Destination
	{
		get => new(NPC.ai[2], NPC.ai[3]);
		set
		{
			NPC.ai[2] = value.X;
			NPC.ai[3] = value.Y;
		}
	}

	public ref float StateTimer => ref NPC.localAI[0];
	public ref float StateDuration => ref NPC.localAI[1];

	public Player TargetPlayer;

	// Helper method to determine the minion type
	public static int MinionType()
	{
		return ModContent.NPCType<HomingFood>();
	}

	// Helper method to determine the amount of minions summoned
	public static int MinionCount()
	{
		int count = 15;
		if (Main.expertMode)
			count += 5; // Increase by 5 if expert or master mode
		if (Main.getGoodWorld)
			count += 5; // Increase by 5 if using the "For The Worthy" seed
		return count;
	}

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 4;

		NPCID.Sets.MPAllowedEnemies[Type] = true;
		NPCID.Sets.BossBestiaryPriority.Add(Type);

		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override void SetDefaults()
	{
		NPC.width = 110;
		NPC.height = 110;
		NPC.damage = 12;
		NPC.defense = 10;
		NPC.lifeMax = 2000;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 0f;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.value = Item.buyPrice(gold: 5);
		NPC.boss = true;
		NPC.npcSlots = 10f;
		NPC.aiStyle = -1;

		ContentSamples.NpcBestiaryRarityStars[Type] = 2;

		if (!Main.dedServ)
		{
			Music = MusicID.Boss1;
			if (!Main.swapMusic == Main.drunkWorld && !Main.remixWorld)
				Music = MusicID.OtherworldlyBoss1;
		}
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.Info.AddRange([
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,
			new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Gorgeist")
		]);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GorgeistBossTrophy>(), 10));

		LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

		notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GorgeistMask>(), 7));
		notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SterlingPlatter>(), 3));

		npcLoot.Add(notExpertRule);

		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GorgeistBossBag>()));

		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GorgeistBossRelic>()));

		npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<GorgeistHeart>(), 4));
	}

	public override bool CanHitPlayer(Player target, ref int cooldownSlot)
	{
		cooldownSlot = ImmunityCooldownID.Bosses;
		return true;
	}

	public override void FindFrame(int frameHeight)
	{
		int startFrame = 0;
		int finalFrame = 1;

		if (HasFlag(Flags.SecondPhase))
		{
			startFrame = 2;
			finalFrame = 3;

			if (NPC.frame.Y < startFrame * frameHeight)
				NPC.frame.Y = startFrame * frameHeight;
		}

		int frameSpeed = 5;
		NPC.frameCounter += 0.5f;
		NPC.frameCounter += NPC.velocity.Length() / 10f;
		if (NPC.frameCounter > frameSpeed)
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += frameHeight;

			if (NPC.frame.Y > finalFrame * frameHeight)
				NPC.frame.Y = startFrame * frameHeight;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		for (int i = 0; i < 15; i++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand, 0, 0);
	}

	public bool HasFlag(Flags flag)
	{
		return (BossFlags & flag) != 0;
	}

	public void SetFlag(Flags flag, bool value = true)
	{
		if (value)
			BossFlags |= flag;
		else
			BossFlags &= ~flag;
	}

	public void SwitchState(State state, float ticks = 0f)
	{
		if (ticks == 0f)
			ticks = GetStateDuration(state);
		CurrentState = state;
		StateTimer = 0f;
		StateDuration = ticks;
		SetFlag(Flags.DidAttack, false);
	}

	public WeightedRandom<State> _attacks = new();

	public override void OnSpawn(IEntitySource source)
	{
		SwitchState(State.Idle);

		_attacks.Add(State.TossPlate, 2);
		_attacks.Add(State.TossFood, 1);
		_attacks.Add(State.CirclingPlayer, 1);
	}

	public override void OnKill()
	{
		DownedBossSystem.downedGorgeistBoss = true;
	}

	public override void AI()
	{
		if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			NPC.TargetClosest();

		if (!NPC.HasPlayerTarget)
			return;

		TargetPlayer = Main.player[NPC.target];

		if (TargetPlayer.dead)
		{
			NPC.velocity.Y -= 0.04f;
			NPC.EncourageDespawn(10);
			return;
		}

		if (HasFlag(Flags.SecondPhase))
			Phase2();
		else
			Phase1();

		if (StateTimer < StateDuration)
			StateTimer++;
		else
			OnStateEnd();

		NPC.velocity += (Destination - NPC.Center) * 0.01f;
		NPC.velocity *= 0.9f;
	}

	public static float GetStateDuration(State state) => state switch
	{
		State.Idle => 2f * 60f,
		State.TossPlate => 1f * 60f,
		State.TossFood => 4f * 60f,
		State.CirclingPlayer => 8f * 60f,
		_ => 4f * 60f
	};

	public void OnStateEnd()
	{
		switch (CurrentState)
		{
			case State.Idle:
				SwitchState(_attacks);
				break;
			case State.TossPlate:
			case State.TossFood:
			case State.CirclingPlayer:
				SwitchState(State.Idle);
				break;
			default:
				SwitchState(CurrentState);
				break;
		}
		if (!HasFlag(Flags.SecondPhase) && NPC.life < NPC.lifeMax / 3)
		{
			SetFlag(Flags.SecondPhase);
			SwitchState(State.Enraged);
		}
	}

	public void EyeSparkle()
	{
		Dust dust = Dust.NewDustPerfect(new(NPC.position.X + 71f, NPC.position.Y - 4f), DustID.BlueTorch, NPC.velocity, default, default, 2);
		dust.noGravity = true;

		SoundEngine.PlaySound(WgSounds.Shing, NPC.Center);
	}

	public void ThrowFood(int count, float offsetX = 1f)
	{
		SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

		if (Main.netMode == NetmodeID.MultiplayerClient) // Needed so that we only spawn projectiles on the server
			return;

		int propagateCount = Main.rand.Next(1, count / 2 + 1); // at least 1, at max count / 2

		for (int i = 0; i < count; i++)
		{
			int propagate = i < propagateCount ? 1 : 0;
			Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new(Main.rand.NextFloat(-offsetX, offsetX), 5f), ModContent.ProjectileType<TossedFood>(), NPC.damage / 2, 2f, -1, propagate);
		}
	}

	public void ThrowPlate(float offsetY = 0f, float speedOffset = 1f)
	{
		SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
		if (Main.netMode == NetmodeID.MultiplayerClient) // Needed so that we only spawn projectiles on the server
			return;
		Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new(15f, 5f), ModContent.ProjectileType<TossedPlate>(), NPC.damage / 2, 2f, -1, TargetPlayer.Center.Y + offsetY, speedOffset);
	}

	public void ThrowPlateVariant(int variant, float speedOffset = 1)
	{
		switch (variant)
		{
			case 0: // Triple Volley
				ThrowPlate(-120f, speedOffset);
				ThrowPlate(0f, speedOffset);
				ThrowPlate(120f, speedOffset);
				break;
			case 1: // Quad Volley
				ThrowPlate(-180f, speedOffset);
				ThrowPlate(-60f, speedOffset);
				ThrowPlate(60f, speedOffset);
				ThrowPlate(180f, speedOffset);
				break;
		}
	}

	public void MultiThrow(float volley1 = 1.1f, float volley2 = 0.95f, float volley3 = 0.8f)
	{
		switch (Main.rand.Next(0, 3))
		{
			case 0:
				ThrowPlateVariant(1, volley1); // Quad
				ThrowPlateVariant(0, volley2); // Triple
				ThrowPlateVariant(1, volley3); // Quad
				break;
			case 1:
				ThrowPlateVariant(1, volley1); // Quad
				ThrowPlate(0f, volley2); // Single
				ThrowPlate(0f, volley3); // Single
				break;
			case 2:
				ThrowPlateVariant(0, volley1); // Triple
				ThrowPlate(0, volley2); // Single
				ThrowPlateVariant(1, volley3); // Quad
				break;
		}
	}

	public void Phase1()
	{
		Destination = TargetPlayer.Center + new Vector2(300f, -300f);
		switch (CurrentState)
		{
			case State.Idle:
				if (StateTimer == StateDuration - 30)
					EyeSparkle();
				break;
			case State.TossPlate:
				if (!HasFlag(Flags.DidAttack))
				{
					int value = 0;

					if (NPC.life <= NPC.lifeMax * 0.75)
						value = 1;

					switch (Main.rand.Next(value, value + 2))
					{
						case 0:
							ThrowPlate(); // Single
							break;
						case 1:
							ThrowPlateVariant(0); // Triple
							break;
						case 2:
							MultiThrow(); // Volley
							break;
					}
					SetFlag(Flags.DidAttack);
				}
				break;
			case State.TossFood:
				Destination = TargetPlayer.Center + new Vector2(0f, -250f);
				if (StateTimer > StateDuration * 0.5f && !HasFlag(Flags.DidAttack))
				{
					ThrowFood(4, 7f);
					SetFlag(Flags.DidAttack);
				}
				break;
			case State.CirclingPlayer:
				float t = StateTimer / StateDuration * MathF.Tau * 2f - MathF.PI * 0.5f;
				Destination = TargetPlayer.Center + new Vector2(MathF.Cos(t) * 150f, MathF.Sin(t) * 150f);
				break;
		}
	}

	public void Phase2()
	{
		// TODO
	}
}
