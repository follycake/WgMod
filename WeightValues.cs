using System;
using Terraria.ID;

namespace WgMod;

public static class WeightValues
{
    public static float GetMountScale(int stage)
    {
        return float.Lerp(1f, 1.5f, Math.Clamp(stage / (float)Weight.ImmobileStage, 0f, 1f));
    }

    public static float GetDeathPenalty(int difficulty) => difficulty switch
    {
        PlayerDifficultyID.SoftCore => 0.8f,
        PlayerDifficultyID.MediumCore => 0.85f,
        PlayerDifficultyID.Hardcore => 0.9f,
        _ => 1f
    };

    public static int GetHitboxWidthInTiles(int stage) => stage switch
    {
        4 => 3,
        5 => 4,
        6 => 5,
        7 => 6,
        8 => 7,
        9 => 8,
        _ => 2,
    };

    public static int GetArmStage(int stage) => SpriteSet.Current.GetStage(stage).Arm;
    public static float DrawOffsetX(int stage) => SpriteSet.Current.GetStage(stage).OffsetX;
    public static float DrawOffsetY(int stage) => SpriteSet.Current.GetStage(stage).OffsetY;
}
