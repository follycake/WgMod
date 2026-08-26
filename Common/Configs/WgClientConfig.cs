using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgClientConfig : ModConfig
{
    public static WgClientConfig Instance => ModContent.GetInstance<WgClientConfig>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("Sprites")]
    [CustomModConfigItem(typeof(SpriteSetElement))]
    [DefaultValue(SpriteSet.DefaultSet)]
    public string PlayerSpriteSet;

    [Header("General")]
    public bool DisableWeightGain;

    [DefaultValue(true)]
    public bool UseImperialUnits;

    [Slider, DrawTicks, Range(WeightStage.Regular, WeightStage.Max), DefaultValue(WeightStage.Max)]
    public int StageCap;

    [Header("Visual")]
    [DefaultValue(false)]
    public bool DisableJiggle;

    [DefaultValue(false)]
    public bool DisableUVClothes;

    [DefaultValue(true)]
    public bool ShowCredits;

    [Header("Volume")]
    [Slider, DrawTicks, Increment(10), Range(0, 100), DefaultValue(100)]
    public int BellyVolume;

    [Slider, DrawTicks, Increment(10), Range(0, 100), DefaultValue(100)]
    public int GurgleVolume;

    [Slider, DrawTicks, Increment(10), Range(0, 100), DefaultValue(100)]
    public int MiscVolume;

    public ref int GetVolume(VolumeChannel channel)
    {
        switch (channel)
        {
            case VolumeChannel.Belly:
                return ref BellyVolume;
            case VolumeChannel.Gurgle:
                return ref GurgleVolume;
            default:
                return ref MiscVolume;
        }
    }

    public override void OnChanged()
    {
        SpriteSet.SetCurrent(Mod, PlayerSpriteSet);
    }
}
