using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.Systems;
using WgMod.Content.Projectiles;

namespace WgMod.Content.NPCs.Sanguist;

[AutoloadHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class Sanguist : ModNPC
{
    public override string Texture
    {
        get { return "WgMod/Content/NPCs/Sanguist/Sanguist"; }
    }

    public const string ShopName = "Shop";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 25;

        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.AttackType[Type] = 0;
        NPCID.Sets.AttackTime[Type] = 10;
        NPCID.Sets.AttackAverageChance[Type] = 5;
        NPCID.Sets.HatOffsetY[Type] = 4;

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers =
            new() { Velocity = -1f, Direction = -1 };

        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
            .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Hate)
            .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Painter, AffectionLevel.Hate);
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 25;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit28;
        NPC.DeathSound = SoundID.NPCDeath31;
        NPC.knockBackResist = 0.5f;

        AnimationType = NPCID.Guide;

        NPC.ApplyTownNPCModifiers();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Sanguist"),
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_SpawnNPC)
        {
            TownNPCRespawnSystem.unlockSanguist = true;
        }
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        if (TownNPCRespawnSystem.unlockSanguist)
            return true;

        return false;
    }

    public override List<string> SetNPCNameList()
    {
        return
        [
            "Dracula", "Serana", "Lilith", "Priscilla", "Mercedes", "Carmilla",
            "Millarca", "Alucard", "Nosferatu", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
        ];
    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new WeightedRandom<string>();


        if (Main.bloodMoon)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.BloodMoonDialogue1")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.BloodMoonDialogue2")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.BloodMoonDialogue3")); // ""
        }
        else if (NPC.homeless)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.HomelessDialogue1")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.HomelessDialogue2")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.HomelessDialogue3")); // ""
        }
        else
        {
            if (Main.IsItRaining)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.RainDialogue1", 2)); // ""
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.RainDialogue2", 2)); // ""
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.RainDialogue3", 2)); // ""
            }
            else if (Main.IsItStorming)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StormDialogue1", 10)); // ""
            else if (Main.dayTime)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.DayDialogue1")); // ""

            if (NPC.loveStruck)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.LoveStruckDialogue1", 10)); // ""

            if (Main.IsItAHappyWindyDay)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WindyDayDialogue1", 2)); // ""
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WindyDayDialogue2", 2)); // ""
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.WindyDayDialogue3", 2)); // ""
            }

            if (BirthdayParty.PartyIsUp)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.PartyDialogue1", 2)); // ""
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.PartyDialogue2", 2)); // ""
            }

            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue1")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue2")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue3")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue4")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue5")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue6")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue7")); // ""
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.StandardDialogue8")); // ""
        }

        return chat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Language.GetTextValue("LegacyInterface.28");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (firstButton)
        {
            shop = ShopName;
        }
    }

    public override void AddShops()
    {
        var sanguistShop = new NPCShop(Type, ShopName)
            .Add(ItemID.BloodbathDye);

        sanguistShop.Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        foreach (Item item in items)
        {
            if (item == null || item.type == ItemID.None)
            {
                continue;
            }
        }
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 30;
        knockback = 4f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 10;
        randExtraCooldown = 5;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ModContent.ProjectileType<HarpyFeatherFriendly>();
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(
        ref float multiplier,
        ref float gravityCorrection,
        ref float randomOffset
    )
    {
        multiplier = 6f;
        randomOffset = 1f;
        gravityCorrection = 5f;
    }
}
