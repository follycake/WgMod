using System;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.Players;
using WgMod.Common.Systems;
using WgMod.Content.Dusts;
using WgMod.Content.Items.Armor.Vanity;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Pets;
using WgMod.Content.Items.Placeable.Furniture;
using WgMod.Content.Items.Weapons.Melee;
using WgMod.Content.Projectiles.Enemy.Gorgeist;

namespace WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;

[AutoloadBossHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.drarky)]
public class Gorgeist : ModNPC
{
	public enum Flags
	{
		None = 0,
		SecondPhase = 1 << 0,
		DidAttack = 1 << 1,
		CircledPlayer = 1 << 2,
		DashUp = 1 << 3
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
		Angry,
		Dash
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

	public int StateTimer;
	public int StateDuration;

	public int DashIndex;
	public int DashCount;

	public Player TargetPlayer;

	public static readonly WeightedRandom<State> Phase1Attacks = new();

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 10;

		NPCID.Sets.MPAllowedEnemies[Type] = true;
		NPCID.Sets.BossBestiaryPriority.Add(Type);

		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

		Phase1Attacks.Clear();
		Phase1Attacks.Add(State.TossPlate, 2);
		Phase1Attacks.Add(State.TossFood, 1);
		Phase1Attacks.Add(State.CirclingPlayer, 1);
	}

	public override void SetDefaults()
	{
		NPC.width = 110;
		NPC.height = 154;
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
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GorgeistTrophy>(), 10));

		LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

		notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GorgeistMask>(), 7));
		notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SterlingPlatter>(), 3));

		npcLoot.Add(notExpertRule);

		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GorgeistBossBag>()));

		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GorgeistRelic>()));

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
		int finalFrame = 3;

		if (HasFlag(Flags.SecondPhase))
		{
			startFrame = 4;
			finalFrame = 7;

			if (NPC.frame.Y < startFrame * frameHeight)
				NPC.frame.Y = startFrame * frameHeight;
		}

		if (CurrentState == State.CirclingPlayer)
		{
			startFrame = 8;
			finalFrame = 9;

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

	public void SwitchState(State state, int ticks = 0)
	{
		if (ticks == 0)
			ticks = GetStateDuration(state);
		if (Enraged())
			ticks /= 2;
		CurrentState = state;
		StateTimer = 0;
		StateDuration = ticks;
		SetFlag(Flags.DidAttack, false);
	}

	public bool Enraged()
	{
		if (TargetPlayer == null || TargetPlayer.ZoneDesert || TargetPlayer.ZoneUndergroundDesert || TargetPlayer.ZoneSandstorm)
			return false;
		return true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		SwitchState(State.Idle);
	}

	public override void OnKill()
	{
		DownedBossSystem.downedGorgeist = true;
	}

	public override void AI()
	{
		if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			NPC.TargetClosest();

		if (!NPC.HasPlayerTarget)
		{
			TargetPlayer = null;
			return;
		}

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

		if (CurrentState == State.Dash)
		{
			NPC.velocity += (Destination - NPC.Center) * 0.1f;
			NPC.velocity *= 0.7f;
		}
		else
		{
			NPC.velocity += (Destination - NPC.Center) * 0.01f;
			NPC.velocity *= 0.9f;
		}
		NPC.spriteDirection = NPC.direction;

		if (Enraged())
		{
			NPC.damage = 24;

			Main.windSpeedTarget = 1f;

			if (!Sandstorm.Happening)
				Sandstorm.StartSandstorm();

			if (!Main.raining && Main.netMode != NetmodeID.MultiplayerClient)
				Main.StartRain(); // TODO: Start a storm rather than just rain
		}
		else
			NPC.damage = 12;
	}

	public static int GetStateDuration(State state) => state switch
	{
		// Phase 1
		State.Idle => 2 * 60,
		State.TossPlate => 1 * 60,
		State.TossFood => 4 * 60,
		State.CirclingPlayer => 8 * 60,

		// Phase 2
		State.Angry => 2 * 60,
		State.Dash => 40,

		// Fallback
		_ => 4 * 60
	};

	public void OnStateEnd()
	{
		switch (CurrentState)
		{
			case State.Idle:
				if (!HasFlag(Flags.SecondPhase))
					SwitchState(Phase1Attacks);
				else
					Dash();
				break;
			case State.TossPlate:
			case State.TossFood:
			case State.CirclingPlayer:
			case State.Angry:
				SwitchState(State.Idle);
				break;
			case State.Dash:
				DashIndex++;
				if (DashIndex >= DashCount)
				{
					SwitchState(State.Idle);
					DashIndex = 0;
				}
				else
				{
					SetFlag(Flags.DashUp, DashIndex % 2 != 0);
					SwitchState(State.Dash);
				}
				break;
			default:
				SwitchState(CurrentState);
				break;
		}
		if (!HasFlag(Flags.SecondPhase) && NPC.life < NPC.lifeMax / 2)
		{
			SetFlag(Flags.SecondPhase);
			SwitchState(State.Angry);
		}
	}

	public WeightedRandom<string> WittyDialogue;

	public void WittyBanter(int type)
	{
		/*switch (type)
		{
			case 0:
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.PlateTossDialogue1"), 1);
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.PlateTossDialogue2"), 1);
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.PlateTossDialogue3"), 1);
				break;
			case 1:
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.FoodTossDialogue1"), 1);
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.FoodTossDialogue2"), 1);
				WittyDialogue.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Gorgeist.FoodTossDialogue3"), 1);
				break;
		}

		Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(WittyDialogue), Color.AliceBlue);*/
	}

	public void EyeSparkle()
	{
		Dust dust = Dust.NewDustPerfect(new(NPC.position.X + 38f, NPC.position.Y + 2f), ModContent.DustType<EyeSparkle>(), NPC.velocity, 50, default, 1);
		dust.noGravity = true;
		SoundEngine.PlaySound(SoundID.MaxMana, NPC.Center);
	}

	public void FacePlayer()
	{
		if (TargetPlayer.Center.X > NPC.Center.X)
			NPC.direction = 1;
		else
			NPC.direction = -1;
	}

	public void Dash(int count = 4)
	{
		DashCount = count;
		SetFlag(Flags.DashUp, false);
		SwitchState(State.Dash);
	}

	public void ThrowFood(int count, float offsetX = 1f)
	{
		SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
		if (Main.netMode == NetmodeID.MultiplayerClient) // Needed so that we only spawn projectiles on the server
			return;

		int propogateFactor = 1;
		if (Main.expertMode)
			propogateFactor = 2;

		int propagateCount = Main.rand.Next(propogateFactor, count / 2 + propogateFactor); // at least 1, at max count / 2
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
				FacePlayer();
				if (StateTimer == StateDuration - 30)
					EyeSparkle();
				break;
			case State.TossPlate:
				FacePlayer();
				SetFlag(Flags.CircledPlayer, false);
				if (!HasFlag(Flags.DidAttack))
				{
					int value = 0;
					if (Main.expertMode)
						value = 1;
					switch (Main.rand.Next(value, value + 1))
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
					WittyBanter(0);
					SetFlag(Flags.DidAttack);
				}
				break;
			case State.TossFood:
				SetFlag(Flags.CircledPlayer, false);
				Destination = TargetPlayer.Center + new Vector2(0f, -250f);
				if (StateTimer > StateDuration * 0.5f && !HasFlag(Flags.DidAttack))
				{
					int count = 4;
					if (Main.expertMode)
						count = 6;
					ThrowFood(count, 7f);
					WittyBanter(1);
					SetFlag(Flags.DidAttack);
				}
				break;
			case State.CirclingPlayer:
				FacePlayer();
				if (HasFlag(Flags.CircledPlayer) && StateTimer == 0)
					SwitchState(State.TossPlate);
				float t = StateTimer / (float)StateDuration * MathF.Tau * 2f - MathF.PI * 0.5f;
				Destination = TargetPlayer.Center + new Vector2(MathF.Cos(t) * (150f + TargetPlayer.width), MathF.Sin(t) * (150f + TargetPlayer.height));
				break;
		}
	}

	public void Phase2()
	{
		switch (CurrentState)
		{
			case State.Idle:
				FacePlayer();
				Destination = TargetPlayer.Center + new Vector2(300f, -300f);
				if (StateTimer == StateDuration - 30)
					EyeSparkle();
				break;
			case State.Dash:
				if (StateTimer == 0)
				{
					int up = HasFlag(Flags.DashUp) ? -1 : 1;
					Destination += new Vector2(NPC.direction * 100f, up * 400f);
				}
				break;
		}
	}

	public override void DrawEffects(ref Color drawColor)
	{
		if (!Enraged())
			return;

		float velocityX = NPC.velocity.X + Main.windSpeedCurrent;
		float velocityY = NPC.velocity.Y;

		drawColor = new(212, 148, 88); // TODO: Apply lighting to her while enraged

		Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand, 0f + velocityX, -7f + velocityY);
		Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand, 5f + velocityX, -5f + velocityY);
		Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Sand, -5f + velocityX, -5f + velocityY);
	}
}
