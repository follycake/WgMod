using System;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace WgMod.Common.Configs;

public class SpriteSetElement : ConfigElement<string>
{
    static string[] Sets => SpriteSet.FoundSets;

    UIAutoScaleTextTextPanel<string> _optionChoice;
    UIPanel _chooserPanel;
    UIGrid _chooserList;

    bool _needsUpdate;
    bool _expanded;

    public override void OnBind()
    {
        base.OnBind();

        _optionChoice = new UIAutoScaleTextTextPanel<string>(Value);
        _optionChoice.SetPadding(0f);
        _optionChoice.Width.Set(120 + 24 + 12, 0f);
        _optionChoice.UseInnerDimensions = true;
        _optionChoice.PaddingLeft = 36;
        _optionChoice.PaddingRight = 6;
        _optionChoice.Height.Set(30, 0f);
        _optionChoice.Left.Set(-4, 0f);
        _optionChoice.HAlign = 1f;

        _optionChoice.Append(new UIImage(UICommon.DropdownIconTexture)
        {
            MarginLeft = -36,
            MarginTop = 0,
            RemoveFloatingPointsFromDrawPosition = true
        });
        Append(_optionChoice);

        _chooserPanel = new UIPanel
        {
            Top = new(30, 0),
            Width = new(-8, 1),
            Left = new(4, 0),
            BackgroundColor = Color.CornflowerBlue,
            // Each is 30 tall, and 5 list padding. 12 panel padding top and bottom minus the final row list padding
            Height = new(19 + (int)Math.Ceiling(Sets.Length / 4f) * 35, 0)
        };
        _chooserList = new UIGrid
        {
            Height = new(30, 1),
            Width = new(0, 1),
            ManualSortMethod = _ => { }
        };
        for (int i = 0; i < Sets.Length; i++)
            _chooserList.Add(CreateButton(Sets[i]));
        _chooserPanel.Append(_chooserList);
    }

    UIAutoScaleTextTextPanel<string> CreateButton(string name)
    {
        string text = name;
        if (name == SpriteSet.DefaultSet)
            text += " (Default)";

        UIAutoScaleTextTextPanel<string> element = new(text);
        element.Width.Set(120, 0f);
        element.Height.Set(30, 0f);
        element.OnLeftClick += (_, _) =>
        {
            Value = name;
            _expanded = false;
            _needsUpdate = true;
        };
        return element;
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        _expanded = !_expanded;
        _needsUpdate = true;

        base.LeftClick(evt);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        if (!_needsUpdate)
            return;
        _needsUpdate = false;

        if (_expanded)
            Append(_chooserPanel);
        else
            _chooserPanel.Remove();

        float newHeight = _expanded ? 30 + _chooserPanel.Height.Pixels + 4 : 30;
        Height.Set(newHeight, 0f);
        if (Parent != null && Parent is UISortableElement)
            Parent.Height.Pixels = newHeight;

        _optionChoice.SetText(Value);
    }
}
