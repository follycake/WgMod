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
        5 => 3,
        6 => 4,
        7 => 5,
        8 => 6,
        9 => 7,
        _ => 2,
    };
}
