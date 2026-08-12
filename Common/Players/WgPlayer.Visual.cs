using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod.Common.Players;

public partial class WgPlayer
{
    internal float _squishRest = 1f;
    internal float _squishPos = 1f;
    internal float _squishVel;
    internal float _bellyOffset;

    internal bool _armSwing;
    internal int _armSwingFrame;
    internal int _armSwingTimer;

    internal readonly WgArmor.Layer[] _armorLayers = new WgArmor.Layer[4];
    internal RenderTarget2D _armorTarget;

    internal Asset<Texture2D> _headOverride;

    internal float _mountOffY;
    internal float _addedGfxOffY;
    float _lastGfxOffY;

    void InitializeVisuals()
    {
        if (Main.dedServ)
            return;
        if (WgArmor.Enabled)
        {
            Main.RunOnMainThread(() =>
            {
                WgArmor.SetupArmorLayers(this);
                WgArmor.Render(Weight.GetStage(), ref _armorTarget, _armorLayers, Player.Male);
            });
        }
    }

    internal void PreUpdateVisuals()
    {
        Player.gfxOffY = _lastGfxOffY;
        if (!Player.mount.Active)
            _mountOffY = 0f;
        _addedGfxOffY = SpriteSet.GetStage(Weight.GetStage()).OffsetY * -Player.gravDir + _mountOffY;
        _headOverride = null;
    }

    internal void PostUpdateVisuals()
    {
        if (_armSwing)
        {
            _armSwingTimer++;
            if (_armSwingTimer >= 10)
            {
                _armSwingTimer = 0;
                _armSwingFrame++;
            }
            if (_armSwingFrame >= 3)
            {
                _armSwing = false;
                _armSwingFrame = 0;
                _armSwingTimer = 0;
            }
        }

        // Can't find a better way to change the draw position
        _lastGfxOffY = Player.gfxOffY;
        Player.gfxOffY += _addedGfxOffY;

        if (Main.dedServ)
            return;
        if (WgArmor.Enabled)
        {
            WgArmor.SetupArmorLayers(this);
            WgArmor.Render(Weight.GetStage(), ref _armorTarget, _armorLayers, Player.Male);
        }
    }

    void UpdateJiggle()
    {
        const float dt = 1f / 60f;
        if (Main.dedServ || WgClientConfig.Instance.DisableJiggle)
        {
            _squishVel = 0f;
            _squishPos = 1f;
        }
        else
        {
            Vector2 vel = Player.velocity;
            vel.Y += _bellyOffset * 0.6f;

            _squishPos += MathF.Abs(vel.X) * 0.005f;
            _squishPos += vel.Y * 0.008f;

            _squishVel += (_squishRest - _squishPos) * 400f * dt;
            _squishVel = float.Lerp(_squishVel, 0f, 1f - MathF.Exp(-6f * dt));
            _squishPos += _squishVel * dt;
            _squishPos = Math.Clamp(_squishPos, 0.5f, 1.5f);
        }
    }

    public void Jiggle(float amount)
    {
        _squishVel += amount;
    }

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        int stage = Weight.GetStage();
        int armStage = SpriteSet.GetStage(stage).Arm;
        foreach (PlayerDrawLayer drawLayer in PlayerDrawLayerLoader.Layers)
        {
            if (drawLayer == PlayerDrawLayers.ArmOverItem && armStage >= 0)
                drawLayer.Hide();
            else if ((drawLayer == PlayerDrawLayers.Skin || drawLayer == PlayerDrawLayers.Torso || drawLayer == PlayerDrawLayers.Leggings) && stage >= WeightStage.MorbidlyObese)
                drawLayer.Hide();
        }
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (Player.isDisplayDollOrInanimate)
            drawInfo.Position.Y += Player.gfxOffY;
        /*if (Player.mount.Active)
        {
            drawInfo.Position.Y += drawInfo.mountOffSet;
            drawInfo.mountOffSet *= WeightValues.GetMountScale(Weight.GetStage());
            drawInfo.Position.Y -= drawInfo.mountOffSet;
        }*/
    }
}
