using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.Systems;

namespace WgMod.Content.NPCs.TownNPCs.OverflowingMimic;

[AutoloadHead]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class OverflowingMimicNPC : ModNPC
{
    public static int _headIndexGold;
    public static int _headIndexIce;
    public static int _headIndexShadow;
    public static int _headIndexGolden;
    public static int _headIndexDeadMan;

    /// <summary> How long it's been since her last jump. </summary>
    public int _jumpCooldown = 0;
    /// <summary> The cooldown between each jump. </summary>
    public static int _jumpCooldownMax = 60;
    /// <summary> Whether or not she wants to slow down. </summary>
    public bool _jumpStop = false;
    /// <summary> Her average speed. </summary>
    public static float _jumpSpeed = 5f;

    static ITownNPCProfile _overflowingMimicProfile;

    public override void Load()
    {
        _headIndexGold = Mod.AddNPCHeadTexture(Type, $"{Texture}_Gold_Head");
        _headIndexIce = Mod.AddNPCHeadTexture(Type, $"{Texture}_Ice_Head");
        _headIndexShadow = Mod.AddNPCHeadTexture(Type, $"{Texture}_Shadow_Head");
        _headIndexGolden = Mod.AddNPCHeadTexture(Type, $"{Texture}_Golden_Head");
        _headIndexDeadMan = Mod.AddNPCHeadTexture(Type, $"{Texture}_DeadMan_Head");
    }

    public override string Texture => "WgMod/Content/NPCs/TownNPCs/OverflowingMimic/OverflowingMimic";

    public static ITownNPCProfile NPCProfile1 { get => _overflowingMimicProfile; set => _overflowingMimicProfile = value; }

    public override bool CanGoToStatue(bool toQueenStatue) => true;

    public const string MimicShop = "Shop";

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 26;

        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.AttackType[Type] = -1;
        NPCID.Sets.HatOffsetY[Type] = 4;

        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Bleeding] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;

        NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new() { Velocity = -1f, Direction = -1 };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        _overflowingMimicProfile = new OverflowingMimicProfile();

        NPC.Happiness.SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
            .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
            .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Love)
            .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Pirate, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 28;
        NPC.height = 28;
        NPC.aiStyle = NPCAIStyleID.Passive;
        NPC.damage = 30;
        NPC.defense = 12;
        NPC.lifeMax = 300;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath6;
        NPC.knockBackResist = 0.7f;
        NPC.housingCategory = 1;

        AnimationType = NPCID.Guide;

        if (Main.masterMode)
        {
            NPC.damage = 90;
            NPC.lifeMax = 900;
            NPC.knockBackResist = 0.76f;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                NPC.damage = 270;
                NPC.defense = 34;
                NPC.lifeMax = 10500;
                NPC.knockBackResist = 0.92f;
            }
            else if (Main.hardMode)
            {
                NPC.damage = 240;
                NPC.lifeMax = 1500;
            }
        }
        else if (Main.expertMode)
        {
            NPC.damage = 60;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0.73f;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                NPC.damage = 180;
                NPC.defense = 34;
                NPC.lifeMax = 7000;
                NPC.knockBackResist = 0.91f;
            }
            else if (Main.hardMode)
            {
                NPC.damage = 160;
                NPC.lifeMax = 1000;
            }
        }
        else if (Main.hardMode)
        {
            NPC.damage = 80;
            NPC.lifeMax = 500;

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                NPC.damage = 90;
                NPC.defense = 34;
                NPC.lifeMax = 3500;
                NPC.knockBackResist = 0.9f;
            }
        }

        NPC.ApplyTownNPCModifiers();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.OverflowingMimic"),
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (source is EntitySource_SpawnNPC)
            TownNPCRespawnSystem.unlockOverflowingMimic = true;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        return TownNPCRespawnSystem.unlockOverflowingMimic;
    }

    public override ITownNPCProfile TownNPCProfile()
    {
        return _overflowingMimicProfile;
    }

    public override List<string> SetNPCNameList()
    {
        return
        [
            "Rack", "Cabinet", "Hutch", "Pantry", "Drawer", "Shelf",
            "Ottoman", "Cassone", "Hope", "Bride", "Trousseau", "Treasure",
            "Trunk", "Wolfert", "Decebalus", "Alaric", "Forrest", "Kidd",
            "Jewel", "Pearl", "Loot", "Gold", "Booty", "Glory",
            "Prize", "Stash", "Cache", "Reserve", "Stock", "Osiris",
            "Ark", "Koshchei", "Gygax", "Greenwood", "Detwiler", "Rowland",
        ];
    }

    public override string GetChat()
    {
        _jumpStop = true;

        WeightedRandom<string> chat = new();

        string overflowingMimic = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<OverflowingMimicNPC>())].GivenName;
        string player = Main.LocalPlayer.name;

        if (Main.bloodMoon)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.BloodMoonDialogue1", overflowingMimic)); // "GRAHAHA! THE MOON HUNGRY TONIGHT!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.BloodMoonDialogue2", overflowingMimic)); // "EAT EVERYTHING! EAT EVERYONE!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.BloodMoonDialogue3", overflowingMimic)); // "OOUUURRRPP..."
        }
        else if (NPC.loveStruck)
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.LoveStruckDialogue1", overflowingMimic, player)); // "{1}... {0} kinda wants to... bite {1} face..."
        else if (NPC.homeless)
        {
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.HomelessDialogue1", overflowingMimic)); // "Mean human! Give {0} a home!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.HomelessDialogue2", overflowingMimic)); // "Can human make {0} really really big chest?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.HomelessDialogue3", overflowingMimic)); // "{0} too fat for chest now! Oh no!"
        }
        else
        {
            if (Main.IsItRaining)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.RainDialogue1", overflowingMimic), 2); // "Rain rain, go away..."
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.RainDialogue2", overflowingMimic), 2); // "{0} so boooooored... wants to play outside!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.RainDialogue3", overflowingMimic, player), 2); // "Does {1} know spell to stop rain?"
            }
            else if (Main.IsItStorming)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StormDialogue1", overflowingMimic), 10); // "Human, help! There giant loud monster outside!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StormDialogue2", overflowingMimic), 10); // "AH! RUN AWAY!"
            }
            else if (Main.dayTime)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.DayDialogue1", overflowingMimic)); // "Pretty day! Full of play!"
            }
            else
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.NightDialogue1", overflowingMimic)); // "{0} love nighttime, so many friends out to play!"
            }

            if (Main.IsItAHappyWindyDay)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.WindyDayDialogue1", overflowingMimic), 2); // "Sky try to steal {0}, but {0} too heavy!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.WindyDayDialogue2", overflowingMimic), 2); // "{0} watch the plants dance all day... so pretty..."
            }

            if (BirthdayParty.PartyIsUp)
            {
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.PartyDialogue1", overflowingMimic), 2); // "{0} tried to take a nap and humans put cups on top of {0}! So rude!"
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.PartyDialogue2", overflowingMimic), 2); // "Woo woo woo! Dance dance!"
            }

            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.PostMechDialogue1", overflowingMimic));

            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue1", overflowingMimic)); // "Why make {0} big, human? Mean human! Bad!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue2", overflowingMimic)); // "Bwah! Don't look at {0}'s belly!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue3", overflowingMimic)); // "{Slime like {0} can ride chest! Look like loot!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue4", overflowingMimic, player)); // "Your name {1}? That name silly! {0} name better!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue5", overflowingMimic)); // "{0} chest is getting so heavy to bounce around in... human fault!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue6", overflowingMimic, player)); // "Hungry... {1}! Give {0} food! Now!"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue7", overflowingMimic, player)); // "Could {1} uhmm... teach {0} how doors work again?"
            chat.Add(Language.GetTextValue("Mods.WgMod.Dialogue.OverflowingMimic.StandardDialogue8", overflowingMimic)); // "Uuurrp... {0} found a little bunny... so yummy!"
        }

        return chat;
    }

    public override void ChatBubblePosition(ref Vector2 position, ref SpriteEffects spriteEffects)
    {
        position.Y -= 18f;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Language.GetTextValue("LegacyInterface.28");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (firstButton)
            shop = MimicShop;
    }
    static Item ItemMult(int type, int times = 2)
    {
        Item item = new(type);
        item.shopCustomPrice = item.value * times;
        return item;
    }

    public override void AddShops()
    {
        var mimicShop = new NPCShop(Type, MimicShop)
            // Remix World
            .Add(ItemMult(ItemID.FlareGun), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.Extractinator), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.BandofRegeneration), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.MagicMirror), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.CloudinaBalloon), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.HermesBoots), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.Mace), Condition.PreHardmode, Condition.RemixWorld)
            .Add(ItemMult(ItemID.ShoeSpikes), Condition.PreHardmode, Condition.RemixWorld)
            // Remix World after either evil boss
            .Add(ItemMult(ItemID.ToySled), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.IceBoomerang), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.IceBlade), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.IceSkates), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.BlizzardinaBottle), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.FlurryBoots), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            .Add(ItemMult(ItemID.SnowballCannon), Condition.PreHardmode, Condition.RemixWorld, Condition.DownedEowOrBoc)
            // Regular world hardmode
            .Add(ItemMult(ItemID.DualHook), Condition.Hardmode)
            .Add(ItemMult(ItemID.MagicDagger), Condition.Hardmode)
            .Add(ItemMult(ItemID.PhilosophersStone), Condition.Hardmode)
            .Add(ItemMult(ItemID.TitanGlove), Condition.Hardmode)
            .Add(ItemMult(ItemID.StarCloak), Condition.Hardmode)
            .Add(ItemMult(ItemID.CrossNecklace), Condition.Hardmode)
            // After any mech boss
            .Add(ItemMult(ItemID.ToySled), Condition.DownedMechBossAny)
            .Add(ItemMult(ItemID.Frostbrand), Condition.DownedMechBossAny)
            .Add(ItemMult(ItemID.FlowerofFrost), Condition.DownedMechBossAny)
            .Add(ItemMult(ItemID.IceBow), Condition.DownedMechBossAny)
            // On a corrupt world after every mech boss
            .Add(ItemMult(ItemID.ClingerStaff), Condition.CorruptWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.DartRifle), Condition.CorruptWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.ChainGuillotines), Condition.CorruptWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.PutridScent), Condition.CorruptWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.WormHook), Condition.CorruptWorld, Condition.DownedMechBossAll)
            // On a Crimson world after every mech boss
            .Add(ItemMult(ItemID.SoulDrain), Condition.CrimsonWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.DartPistol), Condition.CrimsonWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.FetidBaghnakhs), Condition.CrimsonWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.FleshKnuckles), Condition.CrimsonWorld, Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.TendonHook), Condition.CrimsonWorld, Condition.DownedMechBossAll)
            // After every mech boss
            .Add(ItemMult(ItemID.DaedalusStormbow), Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.FlyingKnife), Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.CrystalVileShard), Condition.DownedMechBossAll)
            .Add(ItemMult(ItemID.IlluminantHook), Condition.DownedMechBossAll)
            /* It looks wrong having the semicolon on its own line so I'm adding this to make it less lonely */;

        mimicShop.Register();
    }

    public override void PostAI()
    {
        // v This shit v is supposed to let us know when she wants to stop, but it isn't working because the project file doesn't get rebuilt by Tmodloader
        //NPC.AI_007_FindGoodRestingSpot((int)NPC.position.X / 16, (int)NPC.position.Y / 16, out int floorX, out int floorY);
        //NPC.AI_007_TownEntities_GetWalkPrediction((int)NPC.position.X / 16, floorX, false, false, (int)(NPC.position.X + NPC.width / 2) / 16, (int)(NPC.position.Y + NPC.height + 1) / 16, out bool keepWalking, out bool avoidFalling);

        ///*
        if (_jumpCooldown < _jumpCooldownMax)
            _jumpCooldown++;
        else if (NPC.velocity.X != 0 && NPC.velocity.Y == 0)
        {
            NPC.velocity.Y = -4f * Main.rand.NextFloat(0.85f, 1.15f);
            NPC.velocity.X = NPC.direction * _jumpSpeed * Main.rand.NextFloat(0.5f, 2f);

            _jumpCooldown = Main.rand.Next(0, 21);
        }

        if (NPC.velocity.Y != 0)
        {
            if (_jumpStop)
            {
                NPC.velocity.X *= 0.5f;

                _jumpStop = false;
            }
            else
                NPC.velocity.X *= 0.9f;
        }

        if (NPC.velocity.Y == 0 && NPC.velocity.X != 0)
            NPC.velocity.X = 0.1f * NPC.direction;  //*/
    }
}

public class OverflowingMimicProfile : ITownNPCProfile
{
    static readonly string _filePath = "WgMod/Content/NPCs/TownNPCs/OverflowingMimic/OverflowingMimic";

    readonly Asset<Texture2D> _variantWood = ModContent.Request<Texture2D>(_filePath);
    readonly Asset<Texture2D> _variantGold = ModContent.Request<Texture2D>($"{_filePath}_Gold");
    readonly Asset<Texture2D> _variantIce = ModContent.Request<Texture2D>($"{_filePath}_Ice");
    readonly Asset<Texture2D> _variantShadow = ModContent.Request<Texture2D>($"{_filePath}_Shadow");
    readonly Asset<Texture2D> _variantGolden = ModContent.Request<Texture2D>($"{_filePath}_Golden");
    readonly Asset<Texture2D> _variantDeadMan = ModContent.Request<Texture2D>($"{_filePath}_DeadMan");
    readonly int _headIndexWood = ModContent.GetModHeadSlot($"{_filePath}_Head");

    public int RollVariation()
    {
        int random = Main.rand.Next(6);

        return random;
    }


    public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
    {
        return npc.townNpcVariationIndex switch
        {
            0 => _variantWood,
            1 => _variantGold,
            2 => _variantShadow,
            3 => _variantIce,
            4 => _variantGolden,
            5 => _variantDeadMan,
            _ => _variantWood
        };
    }

    public int GetHeadTextureIndex(NPC npc)
    {
        return npc.townNpcVariationIndex switch
        {
            0 => _headIndexWood,
            1 => OverflowingMimicNPC._headIndexGold,
            2 => OverflowingMimicNPC._headIndexShadow,
            3 => OverflowingMimicNPC._headIndexIce,
            4 => OverflowingMimicNPC._headIndexGolden,
            5 => OverflowingMimicNPC._headIndexDeadMan,
            _ => _headIndexWood
        };
    }

    public string GetNameForVariant(NPC npc) => npc.getNewNPCName();
}
