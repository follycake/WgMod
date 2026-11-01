using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WgMod.Common.Configs;

public class WgServerConfig : ModConfig
{
    public static WgServerConfig Instance => ModContent.GetInstance<WgServerConfig>();
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Header("General")]
    [DefaultValue(false)]
    public bool DisableFatBuffs;

    [DefaultValue(false)]
    public bool DisableFatHitbox;

    [DefaultValue(false)]
    public bool DisablePlayerPushing;
}
