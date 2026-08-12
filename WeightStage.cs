namespace WgMod;

public static class WeightStage
{
    public const int Count = 10; // The total amount of stages and player sprites
    public const int Max = Count - 1; // The last weight stage

    public const int Regular = 0;
    public const int Chubby = 1;
    public const int Overweight = 2;
    public const int Fat = 3;
    public const int Obese = 4;
    public const int MorbidlyObese = 5;
    public const int BarelyMobile = 6;
    public const int Immobile = 7; // Stage at which the player would be considered immobile under normal conditions
    public const int Encumbered = 8;
    public const int Blob = 9; // Stage at which the player can no longer move their arms

    public const int ForcedImmobile = Encumbered; // Stage at which the player will no longer move, at all
    public const int DamageReduction = Overweight; // Stage at which damage reduction starts being applied
    public const int Heavy = Fat; // Stage at which thin ice breaks, max life starts being increased
}
