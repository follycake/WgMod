using Terraria.ID;

namespace WgMod;

partial class WgMod
{
    static GainOptions GainOverTime(float totalGain, float totalTime) => new(totalGain, totalTime);

    void RegisterBuffs()
    {
        // Vanilla
        AddBuffs([
            (BuffID.WellFed, 3f),
            (BuffID.WellFed2, 5f),
            (BuffID.WellFed3, 7f),
            (BuffID.ObsidianSkin, 2f),
            (BuffID.Regeneration, 6f),
            (BuffID.Swiftness, 1f),
            (BuffID.Gills, 1f),
            (BuffID.Ironskin, 3f),
            (BuffID.ManaRegeneration, 3f),
            (BuffID.MagicPower, 2f),
            (BuffID.Featherfall, 1f),
            (BuffID.Spelunker, 5f),
            (BuffID.Invisibility, 1f),
            (BuffID.Shine, 1f),
            (BuffID.NightOwl, 1f),
            (BuffID.Battle, 1f),
            (BuffID.WaterWalking, 1f),
            (BuffID.Warmth, 10f),
            (BuffID.Thorns, 1f),
            (BuffID.Archery, 1f),
            (BuffID.Hunter, 2f),
            (BuffID.Gravitation, 1f),
            (BuffID.PotionSickness, 8f),
            (BuffID.Tipsy, 10f),
            (BuffID.Ichor, 1f),
            (BuffID.ManaSickness, 5f),
            (BuffID.Mining, 1f),
            (BuffID.Heartreach, 2f),
            (BuffID.Calm, 5f),
            (BuffID.Builder, 1f),
            (BuffID.Titan, 6f),
            (BuffID.Flipper, 1f),
            (BuffID.Summoning, 2f),
            (BuffID.Dangersense, 1f),
            (BuffID.AmmoReservation, 1f),
            (BuffID.Lifeforce, 5f),
            (BuffID.Endurance, 3f),
            (BuffID.Rage, 1f),
            (BuffID.Inferno, 2f),
            (BuffID.Wrath, 3f),
            (BuffID.Fishing, 1f),
            (BuffID.Sonar, 1f),
            (BuffID.Crate, 1f),
            (BuffID.SoulDrain, 1f),
            (BuffID.SugarRush, 12f),
            (BuffID.Lucky, 1f),
            (BuffID.Slimed, GainOverTime(2f, 1f)),
            (BuffID.HeartyMeal, GainOverTime(2f, 1f)),
            (BuffID.Honey, GainOverTime(3f, 1f)),
            (BuffID.SoulDrain, GainOverTime(3f, 1f)),
        ]);

        // WgMod
        AddBuffs(
            "WgMod",
            [
                ("AmbrosiaGorged", 6f),
                ("WobWobWobWob", 6f),
                ("GnomeLuck", 6f),
                ("SpikedSkin", 6f),
                ("Caramel", 12f),
                ("CrispyDebuff", 4f),
            ]
        );

        // Calamity Mod
        AddBuffs(
            "CalamityMod",
            [
                ("BloodyMaryBuff", 6f),
                ("CaribbeanRumBuff", 6f),
                ("CinnamonRollBuff", 8f),
                ("EverclearBuff", 6f),
                ("EvergreenGinBuff", 6f),
                ("FireballBuff", 6f),
                ("GrapeBeerBuff", 6f),
                ("MargaritaBuff", 6f),
                ("MoonshineBuff", 6f),
                ("MoscowMuleBuff", 6f),
                ("OldFashionedBuff", 6f),
                ("PurpleHazeBuff", 6f),
                ("RedWineBuff", 6f),
                ("RumBuff", 6f),
                ("ScrewdriverBuff", 6f),
                ("StarBeamRyeBuff", 6f),
                ("TequilaBuff", 6f),
                ("VodkaBuff", 6f),
                ("WhiskeyBuff", 6f),
                ("WhiteWineBuff", 2f),
                ("AnechoicCoatingBuff", 2f),
                ("BaguetteBuff", 6f),
                ("BloodfinBoost", 3f),
                ("BoundingBuff", 2f),
                ("CalciumBuff", 1f),
                ("CeaselessHunger", 12f),
                ("GravityNormalizerBuff", 6f),
                ("Omniscience", 6f),
                ("PhotosynthesisBuff", 4f),
                ("ShadowBuff", 1f),
                ("Soaring", 2f),
                ("SulphurskinBuff", 3f),
                ("Zen", 6f),
                ("Zerg", 6f),
                ("AbsorberRegen", 2f),
                ("GreenJellyRegen", GainOverTime(3f, 1f)),
                ("PinkJellyRegen", GainOverTime(3f, 1f)),
            ]
        );

        // Better Potions
        AddBuffs(
            "BetterPotions",
            [
                ("BerserkerBuff", 3f),
                ("DeterringBuff", 3f),
                ("DiscoInfernoBuff", 6f),
                ("FlightBuff", 3f),
                ("HeightenedSensesBuff", 5f),
                ("ImmovableBuff", 3f),
                ("InstigatingBuff", 3f),
                ("LeapingBuff", 1f),
                ("OrichalcumskinBuff", 5f),
                ("PiercingBuff", 3f),
                ("PredatorBuff", 1f),
                ("SteelfallBuff", 8f),
                ("WarBuff", 3f),
            ]
        );
    }
}
