using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Common.Players;

public partial class BuffHitPlayer
{
    readonly int _slimesBuff = BuffID.Slimed;

    readonly HashSet<int> _slimes =
    [
        NPCID.BlueSlime,
        NPCID.GreenSlime,
        NPCID.RedSlime,
        NPCID.PurpleSlime,
        NPCID.RedSlime,
        NPCID.Pinky,
        NPCID.Slimer,
        NPCID.Slimer2,
        NPCID.YellowSlime,
        NPCID.BlackSlime,
        NPCID.IceSlime,
        NPCID.SandSlime,
        NPCID.JungleSlime,
        NPCID.SpikedIceSlime,
        NPCID.SpikedJungleSlime,
        NPCID.MotherSlime,
        NPCID.BabySlime,
        NPCID.LavaSlime,
        NPCID.DungeonSlime,
        NPCID.GoldenSlime,
        NPCID.KingSlime,
        NPCID.SlimeSpiked,
        NPCID.UmbrellaSlime,
        NPCID.ShimmerSlime,
        NPCID.ToxicSludge,
        NPCID.CorruptSlime,
        NPCID.Slimeling,
        NPCID.Crimslime,
        NPCID.IlluminantSlime,
        NPCID.RainbowSlime,
        NPCID.QueenSlimeBoss,
        NPCID.SlimeRibbonGreen,
        NPCID.SlimeRibbonRed,
        NPCID.SlimeRibbonWhite,
        NPCID.SlimeRibbonYellow,
        NPCID.QueenSlimeMinionBlue,
        NPCID.QueenSlimeMinionPink,
        NPCID.QueenSlimeMinionPurple,
        NPCID.HoppinJack,
        NPCID.SlimedZombie,
    ];

    readonly HashSet<int> _slimeProjectiles =
    [
        ProjectileID.SpikedSlimeSpike,
        ProjectileID.QueenSlimeGelAttack,
        ProjectileID.QueenSlimeMinionBlueSpike,
        ProjectileID.QueenSlimeMinionPinkBall,
        ProjectileID.VolatileGelatinBall,
    ];

    readonly int _beesBuff = ModContent.BuffType<Bloated>();

    readonly HashSet<int> _bees =
    [
        NPCID.Bee,
        NPCID.BeeSmall,
        NPCID.QueenBee,
        NPCID.Hornet,
        NPCID.HornetFatty,
        NPCID.HornetHoney,
        NPCID.HornetLeafy,
        NPCID.HornetStingy,
        NPCID.BigHornetFatty,
        NPCID.BigHornetFatty,
        NPCID.BigHornetLeafy,
        NPCID.BigHornetSpikey,
        NPCID.BigHornetStingy,
        NPCID.MossHornet,
        NPCID.BigMossHornet,
        NPCID.TinyMossHornet,
        NPCID.GiantMossHornet,
        NPCID.LittleMossHornet,
        NPCID.LittleHornetFatty,
        NPCID.LittleHornetHoney,
        NPCID.LittleHornetLeafy,
        NPCID.LittleHornetSpikey,
        NPCID.LittleHornetStingy,
        NPCID.VortexHornet,
        NPCID.VortexHornetQueen,
    ];

    readonly HashSet<int> _bloaters =
    [
        ProjectileID.Stinger,
        ProjectileID.HornetStinger,
        ProjectileID.QueenBeeStinger,
        ProjectileID.Bee,
        ProjectileID.BeeArrow,
        ProjectileID.GiantBee,
        ProjectileID.BeeHive,
        ProjectileID.SporeCloud,
        ProjectileID.SporeTrap,
        ProjectileID.SporeTrap2,
        ProjectileID.SporeGas,
        ProjectileID.SporeGas2,
        ProjectileID.SporeGas3,
        ProjectileID.ToxicCloud,
        ProjectileID.ToxicCloud2,
        ProjectileID.ToxicCloud3,
        ProjectileID.TruffleSpore,
        ProjectileID.GasTrap,
        ProjectileID.JungleSpike,
        ProjectileID.SeedPlantera,
        ProjectileID.PoisonSeedPlantera,
        ProjectileID.ThornBall,
        ProjectileID.DandelionSeed,
    ];

    readonly int _feedersBuff = ModContent.BuffType<ForceFed>();

    readonly HashSet<int> _feeders =
    [
        NPCID.Demon,
        NPCID.FireImp,
        NPCID.Nymph,
        NPCID.VoodooDemon,
        NPCID.CultistArcherBlue,
        NPCID.CultistBoss,
        NPCID.CultistDragonHead,
        NPCID.CultistDragonBody1,
        NPCID.CultistDragonBody2,
        NPCID.CultistDragonBody3,
        NPCID.CultistDragonBody4,
        NPCID.CultistDragonTail,
        NPCID.AncientCultistSquidhead,
        NPCID.FloatyGross,
        NPCID.DesertLamiaDark,
        NPCID.DesertLamiaLight,
        NPCID.RedDevil,
        NPCID.TheBride,
        NPCID.TheGroom,
        NPCID.SandElemental,
        NPCID.GoblinSummoner,
        NPCID.GrayGrunt,
        NPCID.BrainScrambler,
        NPCID.GigaZapper,
        NPCID.MartianDrone,
        NPCID.MartianEngineer,
        NPCID.MartianOfficer,
        NPCID.MartianWalker,
        NPCID.RayGunner,
        NPCID.ScutlixRider,
        NPCID.MartianTurret,
        NPCID.MartianProbe,
        NPCID.MartianSaucer,
        NPCID.MartianSaucerCore,
        NPCID.MartianSaucerTurret,
        NPCID.Poltergeist,
        NPCID.Plantera,
        NPCID.PlanterasHook,
        NPCID.PlanterasTentacle,
        NPCID.Harpy,
        NPCID.ShadowFlameApparition,
        NPCID.AncientLight,
        NPCID.AncientDoom,
        NPCID.BurningSphere,
        NPCID.PresentMimic,
    ];

    readonly HashSet<int> _feederProjectiles =
    [
        ProjectileID.DemonSickle,
        ProjectileID.DemonScythe,
    ];

    readonly int _empressBuff = ModContent.BuffType<PrismaticStuffing>();

    readonly HashSet<int> _empressOfLight =
    [
        ProjectileID.HallowBossRainbowStreak,
        ProjectileID.HallowBossLastingRainbow,
        ProjectileID.FairyQueenLance,
        ProjectileID.FairyQueenSunDance
    ];

    void AddModNPCs()
    {
        AddNPCs(_slimes, "Consolaria", "ShadowSlime");
        AddNPCs(_bees, "Consolaria", "DragonHornet");
        AddNPCs(_feeders, "Consolaria",
            "TurkortheUngrateful",
            "TurkorNeck",
            "TurkortheUngratefulHead",
            "ArchDemon"
        );

        AddNPCs(_slimes, "CalamityMod",
            "PerennialSlime",
            "AeroSlime",
            "CorruptSlimeSpawn",
            "CorruptSlimeSpawn2",
            "CrimsonSlimeSpawn",
            "CrimsonSlimeSpawn2",
            "CrimulanPaladin",
            "EbonianPaladin",
            "SplitCrimulanPaladin",
            "SplitEbonianPaladin",
            "SlimeGodCore",
            "AstralSlime",
            "InfernalCongealment",
            "CryoSlime",
            "BloomSlime",
            "IrradiatedSlime"
        );

        AddNPCs(_feeders, "CalamityMod",
            "WulfrumAmplifier",
            "WulfrumDrone",
            "WulfrumRover",
            "WulfrumGyrator",
            "WulfrumAmplifier",
            "WulfrumHovercraft",
            "SupremeCalamitas",
            "SepulcherArm",
            "BrimstoneHeart",
            "SepulcherBody",
            "SepulcherBodyEnergyBall",
            "SepulcherHead",
            "SepulcherTail",
            "SoulSeekerSupreme",
            "SupremeCataclysm",
            "SupremeCatastrophe",
            "CloudElemental",
            "Brimling",
            "BrimstoneElemental",
            "Anahita",
            "AnahitaIceShield",
            "AquaticAberration",
            "Leviathan",
            "LeviathanStart",
            "Eidolist",
            "OverloadedSoldier",
            "RenegadeWarlock"
        );

        AddNPCs(_feeders, "CalamityFables",
            "WulfrumGrappler",
            "WulfrumMagnetizer",
            "WulfrumMortar",
            "WulfrumNexus",
            "WulfrumRoller",
            "WulfrumRover"
        );
    }
}
