using Terraria.ID;

namespace WgMod;

public static class WeightValues
{
    public static float GetMountScale(int stage)
    {
        return float.Lerp(1f, 1.5f, stage / (float)WeightStage.SoftImmobile);
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
}
