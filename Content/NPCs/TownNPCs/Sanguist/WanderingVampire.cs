using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using WgMod.Content.Dusts;

namespace WgMod.Content.NPCs.TownNPCs.Sanguist;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
[Credit(ProjectRole.Idea, Contributor.igobee_)]
public class WanderingVampire : ModNPC
{
	static Profiles.StackedNPCProfile _nPCProfile;

	const float ConfusionRadius = 600;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 25;

		NPCID.Sets.ExtraFramesCount[Type] = 9;
		NPCID.Sets.AttackFrameCount[Type] = 4;
		NPCID.Sets.AttackType[Type] = -1;
		NPCID.Sets.AttackTime[Type] = 60;
		NPCID.Sets.AttackAverageChance[Type] = 30;
		NPCID.Sets.HatOffsetY[Type] = 4;
		NPCID.Sets.ShimmerTownTransform[NPC.type] = false;

		NPCID.Sets.ActsLikeTownNPC[Type] = true;
		NPCID.Sets.NoTownNPCHappiness[Type] = true;
		NPCID.Sets.SpawnsWithCustomName[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.friendly = true;
		NPC.width = 18;
		NPC.height = 40;
		NPC.aiStyle = NPCAIStyleID.Passive;
		NPC.damage = 10;
		NPC.defense = 15;
		NPC.lifeMax = 250;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.knockBackResist = 0.5f;

		AnimationType = NPCID.Guide;
	}

	public override bool CanChat()
	{
		return true;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		int num = NPC.life > 0 ? 1 : 5;

		for (int k = 0; k < num; k++)
		{
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
		}
	}

	public override ITownNPCProfile TownNPCProfile()
	{
		return _nPCProfile;
	}

	public override List<string> SetNPCNameList()
	{
		return [
				"Cloaked Woman"
			];
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!spawnInfo.Player.ZoneForest || !Main.bloodMoon || TownNPCRespawnSystem.unlockSanguist || NPCUtility.Exists<WanderingVampire>())
			return 0f;

		return 0.34f;
	}

	public override string GetChat()
	{
		WeightedRandom<string> chat = new();

		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue1"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue2"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue3"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue4"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue5"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue6"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue7"));
		chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WanderingDialogue8"));
		return chat;
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		button = Language.GetTextValue("Mods.WgMod.Interface.Offer");
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shop)
	{
		WeightedRandom<string> chat = new();
		if (firstButton)
			if (Main.LocalPlayer.inventory.Any(item => item.type == ItemID.Umbrella) || Main.LocalPlayer.inventory.Any(item => item.type == ItemID.TragicUmbrella))
			{
				NPC.NewNPC(
					Terraria.Entity.GetSource_TownSpawn(),
					(int)NPC.Center.X,
					(int)NPC.Center.Y,
					ModContent.NPCType<SanguistNPC>()
				);
				NPC.active = false;
			}
			else
			{
				Main.npcChatText = Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.DenyOfferDialogue1");
			}
	}

	public override void PostAI()
	{
		if (Main.IsItDay())
		{
			NPC.active = false;

			Dust.NewDust(NPC.position, 0, 0, ModContent.DustType<Bat>(), 0, -4, 0, default, 2);
			for (int i = 0; i < 24; i++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Cloud, 0f, 0f, 100, default, 1);
			}
		}

		for (int i = 0; i < 200; i++)
		{
			NPC npc = Main.npc[i];
			if (
				npc.active
				&& !npc.friendly
				&& npc.damage > 0
				&& !npc.dontTakeDamage
				&& !npc.buffImmune[BuffID.Confused]
				&& Vector2.Distance(NPC.Center, npc.Center) <= ConfusionRadius
			)
			{
				npc.AddBuff(BuffID.Confused, 2 * 60);
			}
		}
	}

	public override void DrawEffects(ref Color drawColor)
	{
		int dustRate = 5;
		float dustIntensity = 3f;
		float dustSize = 1f;

		float sanguistX = NPC.Center.X;
		float sanguistY = NPC.Center.Y;

		for (int i = 0; i < dustRate; i++)
		{
			float angle = (float)(Main.rand.NextDouble() * Math.Tau);
			Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));
			float length = Main.rand.NextFloat() * ConfusionRadius;

			int dust = Dust.NewDust(
				new Vector2(sanguistX + dir.X * ConfusionRadius, sanguistY + dir.Y * ConfusionRadius),
				4,
				4,
				DustID.Shadowflame,
				0f,
				0f,
				(int)(dustIntensity * 20),
				default,
				dustSize * 1.333333f
			);
			int dust2 = Dust.NewDust(
				new Vector2(sanguistX + dir.X * ConfusionRadius * 0.666666f, sanguistY + dir.Y * ConfusionRadius * 0.666666f),
				4,
				4,
				DustID.Shadowflame,
				0f,
				0f,
				(int)(dustIntensity * 20),
				default,
				dustSize
			);
			int dust3 = Dust.NewDust(
				new Vector2(sanguistX + dir.X * ConfusionRadius * 0.333333f, sanguistY + dir.Y * ConfusionRadius * 0.333333f),
				4,
				4,
				DustID.Shadowflame,
				0f,
				0f,
				(int)(dustIntensity * 20),
				default,
				dustSize * 0.666666f
			);
			int dust4 = Dust.NewDust(
				new Vector2(sanguistX + dir.X * length, sanguistY + dir.Y * length),
				4,
				4,
				DustID.Grubby,
				0f,
				0f,
				(int)(dustIntensity * 20),
				default,
				dustSize * 0.666666f
			);

			Main.dust[dust].noGravity = true;
			Main.dust[dust2].noGravity = true;
			Main.dust[dust3].noGravity = true;
			Main.dust[dust4].noGravity = true;
			Main.dust[dust].velocity = NPC.velocity;
			Main.dust[dust2].velocity = NPC.velocity;
			Main.dust[dust3].velocity = NPC.velocity;
			Main.dust[dust4].velocity = NPC.velocity;
		}
	}
}
