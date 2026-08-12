using WgMod.Common.Configs;

namespace WgMod;

// Mass in kg.
public readonly record struct Mass(float Value)
{
    public const float KgToPounds = 2.2046226218f;

    public override string ToString() => Value.ToString();

    public string Display()
    {
        if (WgClientConfig.Instance.UseImperialUnits)
            return $"{ToPounds():0.#} lbs ({Value:0.#} kg)";
        return $"{Value:0.#} kg ({ToPounds():0.#} lbs)";
    }

    public string ShortDisplay()
    {
        if (WgClientConfig.Instance.UseImperialUnits)
            return $"{ToPounds():0.#} lbs";
        return $"{Value:0.#} kg";
    }

    public readonly float ToPounds() => Value * KgToPounds;
    public static Mass FromPounds(float pounds) => new(pounds / KgToPounds);

    public static Mass operator +(Mass a, Mass b) => new(a.Value + b.Value);
    public static Mass operator -(Mass a, Mass b) => new(a.Value - b.Value);

    public static implicit operator float(Mass mass) => mass.Value;
    public static implicit operator Mass(float mass) => new(mass);
}
