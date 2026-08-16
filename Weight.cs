using System;

namespace WgMod;

public readonly record struct Weight(Mass Mass)
{
    public static readonly Weight Base = new(60f);
    public static readonly Weight SoftImmobile = new(600f);

    public readonly float Immobility => GetFactor(Base, SoftImmobile);
    public readonly float ClampedImmobility => GetClampedFactor(Base, SoftImmobile);

    public override readonly string ToString() => Mass.Display();
    public readonly int GetStage() => (int)MathF.Floor(Immobility * WeightStage.SoftImmobile);

    public readonly float GetStageFactor()
    {
        int stage = GetStage();
        float a = FromStage(stage).Mass;
        float b = FromStage(stage + 1).Mass;
        return (Mass - a) / (b - a);
    }

    public readonly float GetFactor(Weight start, Weight end) => Curve((Mass - start.Mass) / (end.Mass - start.Mass)); // Inverese lerp
    public readonly float GetClampedFactor(Weight start, Weight end) => Math.Clamp(GetFactor(start, end), 0f, 1f);

    public readonly float GetFactor(int startStage, int endStage) => GetFactor(FromStage(startStage), FromStage(endStage));
    public readonly float GetClampedFactor(int startStage, int endStage) => GetClampedFactor(FromStage(startStage), FromStage(endStage));

    public static Weight FromStage(int stage)
    {
        if (stage == WeightStage.Regular)
            return Base;
        if (stage == WeightStage.SoftImmobile)
            return SoftImmobile;
        return FromImmobility(stage / (float)WeightStage.SoftImmobile);
    }

    public static Weight FromImmobility(float factor) => new(float.Lerp(Base.Mass, SoftImmobile.Mass, InverseCurve(factor)));

    public static Weight Clamp(Weight weight) => Clamp(weight, WeightStage.Max);
    public static Weight Clamp(Weight weight, int maxStage) => new(Math.Clamp(weight.Mass, Base.Mass, FromStage(maxStage).Mass + 10f));

    public static float Curve(float x) => MathF.Pow(x, 2f / 3f);
    public static float InverseCurve(float x) => MathF.Pow(x, 3f / 2f);

    public static Weight operator +(Weight w, Mass mass) => new(w.Mass + mass);
    public static Weight operator -(Weight w, Mass mass) => new(w.Mass - mass);
}
