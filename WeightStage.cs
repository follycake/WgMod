namespace WgMod;

public static class WeightStage
{
    /// <summary> The total amount of stages and player sprites </summary>
    public const int Count = 10;

    /// <summary> The last weight stage </summary>
    public const int Max = Count - 1;

    public const int Regular = 0;
    public const int Chubby = 1;
    public const int Overweight = 2;
    public const int Fat = 3;
    public const int Obese = 4;
    public const int MorbidlyObese = 5;
    public const int BarelyMobile = 6;
    public const int Encumbered = 7;
    public const int Immobile = 8;
    public const int Blob = 9;

    /// <summary> Stage at which the player would be considered immobile under normal conditions </summary>
    public const int SoftImmobile = Encumbered;

    /// <summary> Stage at which the player will no longer move, at all </summary>
    public const int HardImmobile = Immobile;

    /// <summary> Stage at which damage reduction starts being applied </summary>
    public const int DamageReduction = Overweight;

    /// <summary> Stage at which thin ice breaks, max life starts being increased </summary>
    public const int Heavy = Fat;
}
