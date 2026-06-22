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
using WgMod.Content.Items.Weapons.Ranged;
using WgMod.Content.NPCs.Sanguist;
using WgMod.Content.Projectiles;

namespace WgMod.Content.NPCs.GroundedHarpy;

[AutoloadHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class GroundedHarpyNPC : ModNPC
{
    public override string Texture
    {
        get { return "WgMod/Content/NPCs/GroundedHarpy/GroundedHarpy"; }
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
        NPC.defense = 8;
        NPC.lifeMax = 100;
        NPC.HitSound = SoundID.NPCHit28;
        NPC.DeathSound = SoundID.NPCDeath31;
        NPC.knockBackResist = 0.4f;

        AnimationType = NPCID.Guide;

        if (Main.expertMode)
        {
            NPC.damage = 50;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0.46f;

            if (Main.hardMode)
            {
                NPC.damage = 44;
                NPC.lifeMax = 220;
            }
        }

        if (Main.masterMode)
        {
            NPC.damage = 75;
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0.52f;

            if (Main.hardMode)
            {
                NPC.damage = 66;
                NPC.lifeMax = 330;
            }
        }

        NPC.ApplyTownNPCModifiers();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.GroundedHarpy"),
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_SpawnNPC)
        {
            TownNPCRespawnSystem.unlockGroundedHarpy = true;
        }
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        if (TownNPCRespawnSystem.unlockGroundedHarpy)
            return true;

        return false;
    }

    public override List<string> SetNPCNameList()
    {
        return
        [
            "Keeth", "Orrit", "Elun", "Ari", "Tweety", "Dewey",
            "Archimedes", "Gunter", "Condor", "Quetzal", "Macaw", "Nightingale",
            "Kingfisher", "Hoopoe", "Griffin", "Cockatrice", "Ra", "Thoth",
            "Horus", "Axex", "Zu", "Huma", "Odin", "Cher Ami",
            "Thunderbird", "Ibis", "Raven", "Athena", "Daffy", "Illo",
            "Ceen", "Issot", "Quassice", "Zhonu", "Qhueen", "Obeth",
        ];
    }

    public override string GetChat()
    {
        WeightedRandom<string> chat = new();

        int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
        int sanguist = NPC.FindFirstNPC(ModContent.NPCType<SanguistNPC>());
        int cat = NPC.FindFirstNPC(NPCID.TownCat);
        int dog = NPC.FindFirstNPC(NPCID.TownDog);

        if (Main.bloodMoon)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.BloodMoonDialogue1")); // "Come any closer and I'll use my talons! Craw!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.BloodMoonDialogue2")); // "SCRAW! BACK OFF!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.BloodMoonDialogue3")); // "Huff! Huff! Must lose weight! Scree!"
        }
        else if (NPC.homeless)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.HomelessDialogue1")); // "Human! House me immediately!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.HomelessDialogue2")); // "You did this to me, you take responsibility!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.HomelessDialogue3")); // "Huff... huff... I need somewhere to rest..."
        }
        else
        {
            if (Main.hardMode)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.HardModeDialogue1")); // "I never liked those long dragons... they think harpies are food!"

            if (Main.IsItRaining)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.RainDialogue1", 2)); // "Screech! I can't work out in these conditions!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.RainDialogue2", 2)); // "I guess I'll stay home and eat today..."
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.RainDialogue3", 2)); // "Today's just a cheat day! What? I'm not over my weekly limit!"
            }
            else if (Main.IsItStorming)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StormDialogue1", 10)); // "SCRAW! PANIC! SCRAW!"
            else if (Main.dayTime)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.DayDialogue1")); // "You're interrupting my exercises! I will fly again soon!"

            if (Main.slimeRain)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.SlimeRainDialogue1", 2)); // "How do you think slimes taste?"

            if (NPC.loveStruck)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.LoveStruckDialogue1", 10)); // "Human... you look tastier than usual..."

            if (Main.anglerQuest == ItemID.Cloudfish || Main.anglerQuest == ItemID.Angelfish || Main.anglerQuest == ItemID.Harpyfish || Main.anglerQuest == ItemID.Wyverntail)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.QuestFishDialogue1", 5)); // "No, I won't carry you up to the sky lakes for your fish!"

            if (Main.IsItAHappyWindyDay)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.WindyDayDialogue1", 2)); // "I feel like the wind's gonna take me away!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.WindyDayDialogue2", 2)); // "Screech! I flew! Did you see? What? It was just the wind?"
            }

            if (BirthdayParty.PartyIsUp)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.PartyDialogue1", 2)); // "Human festivities are so fun! And tasty!"

                if (partyGirl >= 0)
                    chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.PartyDialogue2", 2, Main.npc[partyGirl].GivenName)); // "Please tell {NPCName} not to pin the tail on the harpy!"
            }

            if (sanguist >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.SanguistDialogue1", 2, Main.npc[sanguist].GivenName)); // "{1} keeps making fun of me for being fat! She should know it's not my fault!"

            if (cat >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.CatDialogue1", Main.npc[cat].GivenName)); // "Hey... can I watch your cat today? No reason! It just looks so yummy- I mean cute!"

            if (dog >= 0)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.DogDialogue1", Main.npc[dog].GivenName)); // "Your dog barks at me all the time! I'm not a bird!"

            /*if (NPC.downedBoss2)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.EvilBossDialogue1")); // "Scraw! I took the liberty to turn that big evil baddie into food for you! Scraw!" */

            if (!NPC.downedBoss3)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.SkeletronDialogue1")); // "I'd love to make you a traditional harpy stormbow, but I'd need some really big bones to do it!"
            else
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.SkeletronDialogue2")); // "Caw! That skull baddie you killed was perfect for crafting a traditional harpy stormbow!"

            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue1")); // "Scraw! I'm still mad at you for making me this big!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue2")); // "No wonder humans can't fly if all of their food tastes this good!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue4")); // "Have any more of that sweet powder stuff? Why? Mind your business!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue5")); // "Living on the ground is boring, I wanna touch the clouds again!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue6")); // "You've killed some big baddies, did you roast their meat by the fire?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue7")); // "No, we didn't build those sky houses! That's a harmful stereotype, scraw!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.GroundedHarpy.StandardDialogue8")); // "Can you groom my plumage? I've had a hard time doing it myself since... you know!"
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
        var harpyShop = new NPCShop(Type, ShopName)
            .Add(ModContent.ItemType<HarpyStormbow>(), Condition.DownedSkeletron)
            .Add(new Item(ItemID.CreativeWings) { shopCustomPrice = Item.buyPrice(gold: 30) })
            .Add(ItemID.ShinyRedBalloon)
            .Add(ItemID.LuckyHorseshoe)
            .Add(ItemID.CelestialMagnet)
            .Add(ItemID.SkyMill)
            .Add(new Item(ItemID.SunplateBlock) { shopCustomPrice = Item.buyPrice(copper: 50) })
            .Add(new Item(ItemID.Cloud) { shopCustomPrice = Item.buyPrice(copper: 50) })
            .Add(new Item(ItemID.RainCloud) { shopCustomPrice = Item.buyPrice(copper: 50) })
            .Add(ItemID.Feather)
            .Add(ItemID.RainbowBrick, Condition.Hardmode)
            .Add(ItemID.GiantHarpyFeather, Condition.Hardmode)
            .Add(ItemID.SoulofFlight, Condition.DownedMechBossAll)
            .Add(ItemID.SunBanner, Condition.MoonPhaseFull)
            .Add(ItemID.WorldBanner, Condition.MoonPhaseWaningGibbous)
            .Add(ItemID.GravityBanner, Condition.MoonPhaseThirdQuarter)
            .Add(ItemID.SeeTheWorldForWhatItIs, Condition.MoonPhaseWaningCrescent)
            .Add(ItemID.HighPitch, Condition.MoonPhaseNew)
            .Add(ItemID.BlessingfromTheHeavens, Condition.MoonPhaseWaxingCrescent)
            .Add(ItemID.Constellation, Condition.MoonPhaseFirstQuarter)
            .Add(ItemID.LoveisintheTrashSlot, Condition.MoonPhaseWaxingGibbous);

        harpyShop.Register();
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
