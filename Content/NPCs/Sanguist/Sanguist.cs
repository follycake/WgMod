using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using WgMod.Common.Systems;
using WgMod.Content.Items.Weapons.Melee;
using WgMod.Content.NPCs.GroundedHarpy;

namespace WgMod.Content.NPCs.Sanguist;

[AutoloadHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
[Credit(ProjectRole.Idea, Contributor.igobee_)]
public class SanguistNPC : ModNPC
{
    public override string Texture
    {
        get { return "WgMod/Content/NPCs/Sanguist/Sanguist"; }
    }

    public override bool CanGoToStatue(bool toQueenStatue) => toQueenStatue;

    public const int WeightLevelMax = 5;
    public const int WeightProgressMax = 3;
    public const string ShopName = "Shop";

    public int weightLevel;
    public int weightProgress;
    public bool drankToday;

    readonly int _prize1 = ItemID.BloodButcherer;
    readonly int _prize2 = ModContent.ItemType<NightcrawlerSlashes>();
    readonly int _prize3 = ItemID.BloodHamaxe;
    readonly int _prize4 = ItemID.BloodRainBow;

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

        NPC.Happiness.SetBiomeAffection<OceanBiome>(AffectionLevel.Love)
            .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
            .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Nurse, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Like)
            .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate);
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
            "Millarca", "Alucard", "Nosferatu", "Vanilla", "Vio", "Voa",
            "Vanna", "Varcule", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
            "aaa", "aaa", "aaa", "aaa", "aaa", "aaa",
        ];
    }

    static string Belch3(string key, int belchChance)
    {
        string belchVariant = Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.Belches.Belch" + Main.rand.Next(1, 4));
        string belch1 = " ";
        string belch2 = " ";
        string belch3 = " ";
        if (Main.rand.NextBool(belchChance))
            belch1 = belchVariant;
        else if (Main.rand.NextBool(belchChance))
            belch2 = belchVariant;
        else if (Main.rand.NextBool(belchChance))
            belch3 = belchVariant;
        return Language.GetTextValue(key, belch1, belch2, belch3);
    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new();

        int groundedHarpy = NPC.FindFirstNPC(ModContent.NPCType<GroundedHarpyNPC>());

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

            if (groundedHarpy >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Sanguist.HarpyDialogue1", 2, Main.npc[groundedHarpy].GivenName)); // "Gosh! That fat lardy bird brain thinks she's oh so high and mighty!! Calling ME fat!?"

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
        button2 = Language.GetTextValue("Mods.WgMod.Interface.Offer");
    }

    public override bool PreAI()
    {
        if (Main.dayTime && Main.time == 0)
        {
            drankToday = false;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue("Mods.WgMod.Announcements.SanguistHunger"), 255, 22, 22);
            else
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.WgMod.Announcements.SanguistHunger"), new Color(255, 22, 22));
        }
        return true;
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        Player player = Main.LocalPlayer;
        int prevWeightLevel = weightLevel;
        int belchChance = 8 - weightLevel;

        if (firstButton)
        {
            shop = ShopName;
            //weightLevel = 1;
        }
        else if (drankToday == true)
            Main.npcChatText = Belch3("Mods.WgMod.Dialogue.Sanguist.Drink.DrinkDeny2", belchChance);
        else if (weightLevel == WeightLevelMax)
            Main.npcChatText = Belch3("Mods.WgMod.Dialogue.Sanguist.Drink.DrinkDeny3", belchChance);
        else if (player.ConsumedLifeCrystals > 0)
        {
            drankToday = true;
            player.ConsumedLifeCrystals -= 1;

            if (weightProgress < WeightProgressMax)
                weightProgress++;
            else
            {
                weightProgress = 0;
                if (weightLevel < WeightLevelMax)
                    weightLevel++;
            }

            if (weightLevel == prevWeightLevel)
                player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ItemID.GoldCoin);
            else
            {
                switch (weightLevel)
                {
                    case 1:
                        player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), _prize1);
                        break;
                    case 2:
                        player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), _prize2);
                        break;
                    case 3:
                        player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), _prize3);
                        break;
                    case 4:
                        player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), _prize4);
                        break;
                }
            }
            Main.npcChatText = Belch3("Mods.WgMod.Dialogue.Sanguist.Drink.Drink" + (1 + weightLevel) + Main.rand.Next(1, 4), belchChance);
        }
        else
        {
            Main.npcChatText = Belch3("Mods.WgMod.Dialogue.Sanguist.Drink.DrinkDeny1", belchChance);
        }
    }

    public override void AddShops()
    {
        var sanguistShop = new NPCShop(Type, ShopName)
            .Add(ItemID.BloodbathDye)
            .Add(_prize1, new Condition("Mods.WgMod.Conditions.SanguistWeight1", () => weightLevel > 1))
            .Add(_prize2, new Condition("Mods.WgMod.Conditions.SanguistWeight2", () => weightLevel > 2))
            .Add(_prize3, new Condition("Mods.WgMod.Conditions.SanguistWeight3", () => weightLevel > 3))
            .Add(_prize4, new Condition("Mods.WgMod.Conditions.SanguistWeight4", () => weightLevel > 4));

        sanguistShop.Register();
    }

    /*public override void ModifyActiveShop(string shopName, Item[] items)
    {
        foreach (Item item in items)
        {
            if (item == null || item.type == ItemID.None)
            {
                continue;
            }
        }
    }*/

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
        projType = ProjectileID.BloodCloudMoving;
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 6f;
        randomOffset = 1f;
        gravityCorrection = 5f;
    }

    // Saving
    public override void LoadData(TagCompound tag)
    {
        if (!tag.TryGet(nameof(weightLevel), out weightLevel))
            weightLevel = 0;
        if (!tag.TryGet(nameof(weightProgress), out weightProgress))
            weightProgress = 0;
        if (!tag.TryGet(nameof(drankToday), out drankToday))
            drankToday = false;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(weightLevel)] = weightLevel;
        tag[nameof(weightProgress)] = weightProgress;
        tag[nameof(drankToday)] = drankToday;
    }
}
