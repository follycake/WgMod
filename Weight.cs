using System;

namespace WgMod;

// Mass in kg.
public readonly record struct Weight(float Mass)
{
    const float KgToPounds = 2.2046226218f;

    public static readonly Weight Base = new(70f);
    public static readonly Weight Immobile = new(400f);

    public const int StageCount = 8; // The total amount of stages and player sprites
    public const int MaxStage = StageCount - 1; // The last weight stage

    public const int ImmobileStage = 7; // Stage at which the player would be considered immobile under normal conditions
    public const int DamageReductionStage = 2; // Stage at which damage reduction starts being applied
    public const int HeavyStage = 3; // Stage at which thin ice breaks, max life starts being increased

    public readonly float Immobility => GetFactor(Base, Immobile);
    public readonly float ClampedImmobility => GetClampedFactor(Base, Immobile);

    public override readonly string ToString() => $"{Mass} kg";
    public readonly float ToPounds() => Mass * KgToPounds;
    public readonly int GetStage() => (int)MathF.Floor(Immobility * ImmobileStage);

    public readonly float GetStageFactor()
    {
        int stage = GetStage();
        float a = FromStage(stage).Mass;
        float b = FromStage(stage + 1).Mass;
        return (Mass - a) / (b - a);
    }

    public readonly float GetFactor(Weight start, Weight end) => (Mass - start.Mass) / (end.Mass - start.Mass); // Inverese lerp
    public readonly float GetClampedFactor(Weight start, Weight end) => Math.Clamp(GetFactor(start, end), 0f, 1f);

    public static Weight FromStage(int stage) => FromImmobility(stage / (float)ImmobileStage);
    public static Weight FromImmobility(float factor) => new(float.Lerp(Base.Mass, Immobile.Mass, factor));

    public static Weight FromPounds(float pounds) => new(pounds / KgToPounds);
    public static Weight Clamp(Weight weight) => Clamp(weight, MaxStage);
    public static Weight Clamp(Weight weight, int maxStage) => new(Math.Clamp(weight.Mass, Base.Mass, FromStage(maxStage).Mass + 10f));

    public static Weight operator +(Weight w, float mass) => new(w.Mass + mass);
    public static Weight operator -(Weight w, float mass) => new(w.Mass - mass);
}
