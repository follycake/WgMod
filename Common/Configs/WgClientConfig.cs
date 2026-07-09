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
    [DefaultValue(SpriteSet.DefaultSet), ReloadRequired]
    public string PlayerSpriteSet;
}
