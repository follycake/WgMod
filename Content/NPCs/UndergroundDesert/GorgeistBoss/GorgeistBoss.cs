
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Placeable.Furniture;

namespace WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;

[AutoloadBossHead]

public class GorgeistBossBody : ModNPC
{
	// This code here is called a property: It acts like a variable, but can modify other things. In this case it uses the NPC.ai[] array that has four entries.
	// We use properties because it makes code more readable ("if (SecondStage)" vs "if (NPC.ai[0] == 1f)").
	// We use NPC.ai[] because in combination with NPC.netUpdate we can make it multiplayer compatible. Otherwise (making our own fields) we would have to write extra code to make it work (not covered here)
	public bool SecondStage
	{
		get => NPC.ai[0] == 1f;
		set => NPC.ai[0] = value ? 1f : 0f;
	}
	// If your boss has more than two stages, and since this is a boolean and can only be two things (true, false), consider using an integer or enum

	// More advanced usage of a property, used to wrap around to floats to act as a Vector2
	public Vector2 FirstStageDestination
	{
		get => new(NPC.ai[1], NPC.ai[2]);
		set
		{
			NPC.ai[1] = value.X;
			NPC.ai[2] = value.Y;
		}
	}

	public int GorgeistMaxHealthTotal
	{
		get => (int)NPC.ai[3];
		set => NPC.ai[3] = value;
	}

	public int GorgeistHealthTotal { get; set; }

	// Auto-implemented property, acts exactly like a variable by using a hidden backing field
	public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

	// This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
	// Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
	public bool SpawnedMinions
	{
		get => NPC.localAI[0] == 1f;
		set => NPC.localAI[0] = value ? 1f : 0f;
	}

	const int FirstStageTimerMax = 90;
	// This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
	public ref float FirstStageTimer => ref NPC.localAI[1];

	// We could also repurpose FirstStageTimer since it's unused in the second stage, or write "=> ref FirstStageTimer", but then we have to reset the timer when the state switch happens
	public ref float SecondStageTimer_SpawnEyes => ref NPC.localAI[3];

	// Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
	// If you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same variable in different ways), and that you don't accidently use NPC.ai[] directly

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
		{
			count += 5; // Increase by 5 if expert or master mode
		}

		if (Main.getGoodWorld)
		{
			count += 5; // Increase by 5 if using the "For The Worthy" seed
		}

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

		if (!Main.dedServ)
		{
			Music = MusicID.Boss1;

			if (!Main.swapMusic == Main.drunkWorld && !Main.remixWorld)
			{
				Music = MusicID.OtherworldlyBoss1;
			}
		}
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.Info.AddRange([
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
				new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.GorgeistBossBody")
			]);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		// Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

		// The order in which you add loot will appear as such in the Bestiary. To mirror vanilla boss order:
		// 1. Trophy
		// 2. Classic Mode ("not expert")
		// 3. Expert Mode (usually just the treasure bag)
		// 4. Master Mode (relic first, pet last, everything else in between)

		// Trophies are spawned with 1/10 chance
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GorgeistBossTrophy>(), 10));

		// All the Classic Mode drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
		LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

		// Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
		// Boss masks are spawned with 1/7 chance
		//notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GorgeistBossMask>(), 7));

		// This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
		// We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
		// which requires these parameters to be defined
		var parameters = new DropOneByOne.Parameters()
		{
			ChanceNumerator = 1,
			ChanceDenominator = 1,
			MinimumStackPerChunkBase = 1,
			MaximumStackPerChunkBase = 1,
			MinimumItemDropsCount = 12,
			MaximumItemDropsCount = 15,
		};

		//notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters)); // itemType doesn't seem to exist??

		// Finally add the leading rule
		npcLoot.Add(notExpertRule);

		// Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
		//npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GorgeistBossBag>()));

		// ItemDropRule.MasterModeCommonDrop for the relic
		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<GorgeistBossRelic>()));

		// ItemDropRule.MasterModeDropOnAllPlayers for the pet
		//npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<GorgeistBossPetItem>(), 4));
	}

	/* // ImmunityCooldownID.BossNoCheese doesn't seem to exist??
	public override bool CanHitPlayer(Player target, ref int cooldownSlot)
	{
		cooldownSlot = ImmunityCooldownID.BossNoCheese; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
		return true;
	}
	*/

	public override void FindFrame(int frameHeight)
	{
		int startFrame = 0;
		int finalFrame = 1;

		if (SecondStage)
		{
			startFrame = 2;
			finalFrame = 3;

			if (NPC.frame.Y < startFrame * frameHeight)
			{
				NPC.frame.Y = startFrame * frameHeight;
			}
		}

		int frameSpeed = 5;
		NPC.frameCounter += 0.5f;
		NPC.frameCounter += NPC.velocity.Length() / 10f;
		if (NPC.frameCounter > frameSpeed)
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += frameHeight;

			if (NPC.frame.Y > finalFrame * frameHeight)
			{
				NPC.frame.Y = startFrame * frameHeight;
			}
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
	}

	public override void AI()
	{
		if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
		{
			NPC.TargetClosest();
		}

		Player player = Main.player[NPC.target];

		if (player.dead)
		{
			NPC.velocity.Y -= 0.04f;
			NPC.EncourageDespawn(10);
			return;
		}
	}
}
