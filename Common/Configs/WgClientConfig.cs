using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgClientConfig : ModConfig
{
    public static WgClientConfig Instance => ModContent.GetInstance<WgClientConfig>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("General")]
    public bool DisableWeightGain;

    [DefaultValue(true)]
    public bool UseImperialUnits;

    [DefaultValue(WeightStage.Max)]
    [Range(WeightStage.Regular, WeightStage.Max)]
    [Slider]
    [DrawTicks]
    public int StageCap;

    [Header("Visual")]
    [DefaultValue(false)]
    public bool DisableJiggle;

    [DefaultValue(false)]
    public bool DisableUVClothes;

    [DefaultValue(true)]
    public bool ShowCredits;

    [Header("Volume")]
    [DefaultValue(100)]
    [Range(0, 100)]
    [Increment(5)]
    [Slider]
    [DrawTicks]
    public int GurgleVolume;

    [Header("Sprites")]
    [CustomModConfigItem(typeof(SpriteSetElement))]
    [DefaultValue(SpriteSet.DefaultSet)]
    public string PlayerSpriteSet;

    public override void OnChanged()
    {
        SpriteSet.SetCurrent(Mod, PlayerSpriteSet);
    }
}
