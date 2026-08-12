using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace WgMod.Common.Configs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class WorkshopIconsConfig : ModConfig
{
    public static WorkshopIconsConfig Instance => ModContent.GetInstance<WorkshopIconsConfig>();
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("WorkshopIcons")]
    [DefaultValue(1)]
    [Range(1, 1)]
    [Increment(1)]
    [Slider]
    [DrawTicks]
    public int WorkshopIcons;

    [CustomModConfigItem(typeof(WorkshopIconsElement))]
    public string CurrentIcon => "WgMod/Assets/WorkshopIcons/WorkshopIcon" + WorkshopIcons;

    public string GetDescription() => WorkshopIcons switch
    {
        1 => "Grounded Harpy Art by @_d_u_m_m_y_",
        _ => "UwU"
    };
}

[Credit(ProjectRole.Programmer, Contributor.follycake)]
public class WorkshopIconsElement : ConfigElement<string>
{
    UIImage _image;
    UIAutoScaleTextTextPanel<string> _text;
    string _lastValue;

    public override void OnBind()
    {
        base.OnBind();
        _image = new UIImage(ModContent.Request<Texture2D>(Value))
        {
            MarginLeft = 30, // You can use this to move the fucking texture
            MarginTop = 36, // This too
            RemoveFloatingPointsFromDrawPosition = true
        };
        _text = new UIAutoScaleTextTextPanel<string>(WorkshopIconsConfig.Instance.GetDescription());
        _text.SetPadding(0f);
        _text.Width.Set(458, 0f);
        _text.UseInnerDimensions = true;
        _text.PaddingLeft = 6;
        _text.PaddingRight = 6;
        _text.Height.Set(30, 0f);
        _text.Left.Set(-4, 0f);
        _text.HAlign = 1f;
        Append(_text);
        Append(_image);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (Value != _lastValue)
        {
            _text.SetText(WorkshopIconsConfig.Instance.GetDescription());
            _image.SetImage(ModContent.Request<Texture2D>(Value));
        }
        _lastValue = Value;
    }
}
