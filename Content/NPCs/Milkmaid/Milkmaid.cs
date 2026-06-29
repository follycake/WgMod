using System.Collections.Generic;
using Terraria;
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
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;
using WgMod.Content.Items.Placeable.Furniture.Barn;

namespace WgMod.Content.NPCs.Milkmaid;

[AutoloadHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class MilkmaidNPC : ModNPC
{
    public override string Texture
    {
        get { return "WgMod/Content/NPCs/Milkmaid/Milkmaid"; }
    }

    public override bool CanGoToStatue(bool toQueenStatue) => toQueenStatue;

    public static bool milkedToday;

    public const string ShopName = "Shop";

    public int[] LesserWeightPotions = [ModContent.ItemType<LesserWeightGainPotion>(), ModContent.ItemType<LesserWeightLossPotion>()];
    public int[] WeightPotions = [ModContent.ItemType<WeightGainPotion>(), ModContent.ItemType<WeightLossPotion>()];
    public int[] GreaterWeightPotions = [ModContent.ItemType<GreaterWeightGainPotion>(), ModContent.ItemType<GreaterWeightLossPotion>()];
    public int[] SuperWeightPotions = [ModContent.ItemType<SuperWeightGainPotion>(), ModContent.ItemType<SuperWeightLossPotion>()];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 26;

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

        NPC.Happiness.SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
            .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
            .SetBiomeAffection<OceanBiome>(AffectionLevel.Dislike)
            .SetBiomeAffection<DesertBiome>(AffectionLevel.Hate)
            .SetNPCAffection(NPCID.DD2Bartender, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Merchant, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Stylist, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.height = 52;
        NPC.width = 24;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 8;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;

        DrawOffsetY = 4;

        AnimationType = NPCID.Guide;

        if (Main.expertMode)
        {
            NPC.damage = 12;
        }

        if (Main.masterMode)
        {
            NPC.damage = 14;
        }

        NPC.ApplyTownNPCModifiers();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Milkmaid"),
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_SpawnNPC)
        {
            TownNPCRespawnSystem.unlockMilkmaid = true;
        }
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        if (TownNPCRespawnSystem.unlockMilkmaid)
            return true;

        return false;
    }

    public override List<string> SetNPCNameList()
    {
        return
        [
            "Bessie", "Farraday", "Sam", "Clementine", "Daisy", "Beatrice",
            "Princess", "Matilda", "Amber", "Clover", "Ginger", "Hazel",
            "Meadow", "Star", "Buttercup", "Clarabelle", "Maple", "Gertrude",
            "Bella", "Babe", "Gladys", "Otis", "Pauline", "Penny",
            "Ferdinand", "Minos", "Chillingham", "Bagbury", "Helios", "Taurus",
            "Dionysus", "Hera", "Mars", "Neptune", "Vulcan", "Selene",
        ];
    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new();

        int armsDealer = NPC.FindFirstNPC(NPCID.ArmsDealer);
        int dryad = NPC.FindFirstNPC(NPCID.Dryad);
        int mechanic = NPC.FindFirstNPC(NPCID.Mechanic);
        int nurse = NPC.FindFirstNPC(NPCID.Nurse);
        Player player = Main.LocalPlayer;

        if (Main.bloodMoon)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.BloodMoonDialogue1")); // "Hmph."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.BloodMoonDialogue2")); // "Yer face ain't as endearing as it usually is, back up."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.BloodMoonDialogue3")); // "Not enough milk for ya? Too bad."
        }
        else if (NPC.loveStruck)
            if (!player.Male)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.LoveStruckDialogue1")); // "Mroo~ Why don't ya come meet me behind my barn, hun~?"
            else
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.LoveStruckDialogue2")); // "Mmph... I think I'm in heat all of a sudden... Can ya give me some alone time?"
        else if (NPC.homeless)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.HomelessDialogue1")); // "Have ya got a spare barn I can crash in?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.HomelessDialogue2")); // "Yer a hero, save a little farm gal and give her some cheap property."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.HomelessDialogue3")); // "I ain't sure if this land is good fer crops, but with how plump the ladies get it's gonna be great fer animals."
        }
        else
        {
            if (armsDealer >= 0 && nurse >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.ArmsDealerDialogue1", Main.npc[armsDealer].GivenName, Main.npc[nurse].GivenName)); // "Ya'd think {0} would get off my back when I told 'im I was a lesbian, he started talkin' to {1} about hormone replacement."

            if (dryad >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.DryadDialogue1", Main.npc[dryad].GivenName)); // "I offered {0} an apple from one a' my trees, she looked at me like my fat's contagious."

            if (mechanic >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.MechanicDialogue1", Main.npc[mechanic].GivenName)); // "{0} told me she could build a machine to care fer my crops fer me, I told 'er they wouldn't be my crops if I didn't do it myself."

            if (BirthdayParty.PartyIsUp)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.PartyDialogue1"), 2); // "Hey, the frosting on the cake? Yer welcome."

            if (Main.IsItStorming)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StormDialogue1"), 2); // "Jeez, I sure hope my cattle got home safe..."
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StormDialogue2"), 2); // "Whatever god's in charge a the weather, she ain't too joyful."
            }
            else if (Main.IsItRaining)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.RainDialogue1"), 2); // "A lil' rain is good fer the crops, don't mean I gotta like it myself."
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.RainDialogue2"), 2); // "When I was young, ma always told me not to drink the raindrops."
            }
            else if (Main.IsItAHappyWindyDay)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.WindyDayDialogue1"), 2); // "Ahh, the sound of the weather vane always reminds me of better times..."
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.WindyDayDialogue2"), 2); // "Sometimes I like to lay down in a field and feel the wind blow... 's hard to get up afterwards, though."
            }
            else if (Main.dayTime)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.DayDialogue1"), 2); // "Huff, huff... phew! Sun's cookin' me today!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.DayDialogue2"), 2); // "Babydoll, can ya get me somethin' cold to drink? Er, maybe a few things?"
            }

            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue1")); // "I heard there was a hero here who couldn't keep their weight in check. I suppose that's you, doll?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue2")); // "My breasts produce some unique concoctions. One sip'll have ya bedbound or a string bean."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue3")); // "Ya need some milk? I'm yer gal."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue4")); // "Yer a pretty little thing, could use some weight though."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue5")); // "How'd I get this big? Let's see... now was it the custard machine or the feeder cult?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue6")); // "People look at my horns n' think I'm a man. They never heard of a butch."
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue7")); // "Y'know, all the dang monstrosities ya see 'round here, where're all the animals?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid.StandardDialogue8")); // "My damn overalls, they keep shrinkin' or somethin'."
        }

        return chat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = Language.GetTextValue("Mods.WgMod.Interface.Milk");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        Player player = Main.LocalPlayer;

        WeightedRandom<int> potions = new();

        int milkModifier = 0;
        string bloodMoon = "";

        if (firstButton)
        {
            shop = ShopName;
        }
        else
        {
            if (Main.bloodMoon)
                bloodMoon = "Blood";

            if (!milkedToday)
            {
                milkedToday = true;

                // Todo: increase milkModifier in increments of 1 based on certain conditions
                // For instance: Reaching a certain NPC happiness level

                potions.Add(ModContent.ItemType<SuperWeightGainPotion>(), 1 + milkModifier);
                potions.Add(ModContent.ItemType<SuperWeightLossPotion>(), 1 + milkModifier);
                potions.Add(ModContent.ItemType<GreaterWeightGainPotion>(), 2 + milkModifier);
                potions.Add(ModContent.ItemType<GreaterWeightLossPotion>(), 2 + milkModifier);
                potions.Add(ModContent.ItemType<WeightGainPotion>(), 4 - milkModifier);
                potions.Add(ModContent.ItemType<WeightLossPotion>(), 4 - milkModifier);
                potions.Add(ModContent.ItemType<LesserWeightGainPotion>(), 8 - milkModifier);
                potions.Add(ModContent.ItemType<LesserWeightLossPotion>(), 8 - milkModifier);
                potions.Add(ItemID.MilkCarton, 4);

                player.QuickSpawnItem(NPC.GetSource_GiftOrReward(), potions, Main.rand.Next(1, 3 + milkModifier));

                Main.npcChatText = Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid." + bloodMoon + "Milked" + Main.rand.Next(1, 4));

                if (Main.bloodMoon)
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetTextValue("Mods.WgMod.DeathReason.Milkmaid")), Main.rand.Next(25, 101), NPC.direction * -1, false, false, -1, false, 20, 20, 9f);
            }
            else
            {
                Main.npcChatText = Language.GetTextValue("Mods.WgMod.Dialogue.Milkmaid." + bloodMoon + "MilkedDeny");
            }
        }
    }

    public override bool PreAI()
    {

        if (Main.dayTime && Main.time == 0)
        {
            milkedToday = false;
        }

        return true;
    }

    public override void PostAI()
    {
        NPC.velocity.X = NPC.velocity.X * 0.85f;
    }

    public override void AddShops()
    {
        var milkyShop = new NPCShop(Type, ShopName)
            .Add(ModContent.ItemType<BarnWorktable>())
            .Add(ItemID.MilkCarton)
            .Add(ModContent.ItemType<LesserWeightGainPotion>())
            .Add(ModContent.ItemType<LesserWeightLossPotion>())
            .Add(ModContent.ItemType<WeightGainPotion>(), Condition.DownedSkeletron)
            .Add(ModContent.ItemType<WeightLossPotion>(), Condition.DownedSkeletron)
            .Add(ModContent.ItemType<GreaterWeightGainPotion>(), Condition.Hardmode)
            .Add(ModContent.ItemType<GreaterWeightLossPotion>(), Condition.Hardmode)
            .Add(ModContent.ItemType<SuperWeightLossPotion>(), Condition.DownedCultist)
            .Add(ModContent.ItemType<SuperWeightGainPotion>(), Condition.DownedCultist);

        milkyShop.Register();
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
        cooldown = 200;
        randExtraCooldown = 20;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ProjectileID.MolotovCocktail;
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 6f;
        randomOffset = 1f;
        gravityCorrection = 15f;
    }

    // Saving
    public override void LoadData(TagCompound tag)
    {
        if (!tag.TryGet(nameof(milkedToday), out milkedToday))
            milkedToday = false;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(milkedToday)] = milkedToday;
    }
}
