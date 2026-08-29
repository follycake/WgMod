using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod.Common.Players;

public partial class WgPlayer
{
    internal float _squishRest = 1f;
    internal float _squishPos = 1f;
    internal float _squishVel;

    internal float _legOffsetX;
    internal float _legOffsetY;
    internal float _bellyOffset;

    internal bool _fakeWalk;
    internal float _fakeWalkTime;
    internal int _fakeWalkFrameX;
    internal float _fakeWalkJiggle;

    internal readonly WgArmor.Layer[] _armorLayers = new WgArmor.Layer[4];
    internal RenderTarget2D _armorTarget;

    internal List<WgPhysics.Layer> _physicsLayers;
    internal Asset<Texture2D> _headOverride;

    internal float _mountOffY;
    internal float _addedGfxOffY;
    float _lastGfxOffY;

    bool _requestPhysicsSetup;
    static SpriteDrawBuffer _spriteBuffer;

    public override void Load()
    {
        On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += RenderAllLayers;
    }

    public override void Unload()
    {
        On_PlayerDrawLayers.DrawPlayer_RenderAllLayers -= RenderAllLayers;
    }

    void InitializeVisuals()
    {
        if (Main.dedServ)
            return;
        if (WgArmor.Enabled)
        {
            Main.RunOnMainThread(() =>
            {
                WgArmor.SetupArmorLayers(Player, _armorLayers);
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
        // Can't find a better way to change the draw position
        _lastGfxOffY = Player.gfxOffY;
        Player.gfxOffY += _addedGfxOffY;

        if (Main.dedServ)
            return;
        if (WgArmor.Enabled)
        {
            WgArmor.SetupArmorLayers(Player, _armorLayers);
            WgArmor.Render(Weight.GetStage(), ref _armorTarget, _armorLayers, Player.Male);
        }
        if (_requestPhysicsSetup)
        {
            WgPhysics.Setup(this);
            _requestPhysicsSetup = false;
        }
        WgPhysics.Update(this);
    }

    internal void OnSwitchSpriteSet()
    {
        WgPhysics.Setup(this);
    }

    internal void UpdateAnimation()
    {
        _fakeWalk = _finalMovementFactor < 0.01f && (Player.controlLeft || Player.controlRight);
        if (_fakeWalk)
        {
            _fakeWalkTime += 0.2f;
            _fakeWalkTime %= 13f;
            switch ((int)(6f + _fakeWalkTime))
            {
                case 6:
                    _fakeWalkFrameX = 3;
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    _fakeWalkFrameX = 4;
                    break;
                case 11:
                case 12:
                case 13:
                    _fakeWalkFrameX = 3;
                    break;
                case 14:
                    _fakeWalkFrameX = 5;
                    break;
                case 15:
                case 16:
                    _fakeWalkFrameX = 6;
                    break;
                case 17:
                    _fakeWalkFrameX = 5;
                    break;
                case 18:
                case 19:
                    _fakeWalkFrameX = 3;
                    break;
            }
        }
        else
        {
            _fakeWalkTime = 0f;
            _fakeWalkFrameX = 3;
        }

        int frame = Player.legFrame.Y / Player.legFrame.Height;
        if (_fakeWalk)
            frame = (int)(6f + _fakeWalkTime);
        // Frame [0] - Idle
        // Frame [5] - Jump
        // Frame [6 to 19] - Walk

        _legOffsetX = 0f;
        _legOffsetY = 0f;
        _bellyOffset = 0f;
        _fakeWalkJiggle = 0f;
        if (_finalMovementFactor > 0.01f)
        {
            if (frame == 5)
                _bellyOffset = Math.Clamp(Player.velocity.Y * Player.gravDir / 4f, -1f, 1f) * -2f;
            else if (frame >= 6 && frame <= 19)
            {
                float frameTime = (frame - 6) / 13f;
                _legOffsetX = MathF.Sin(frameTime * MathF.Tau) * 2f * Player.direction;
                _legOffsetY = MathF.Max(MathF.Cos(frameTime * MathF.Tau), 0f) * -2f;
                _bellyOffset = MathF.Sin(frameTime * MathF.Tau * 2f) * -2f;
            }
        }
        if (_fakeWalk)
        {
            float frameTime = (frame - 6) / 13f;
            _fakeWalkJiggle = MathF.Sin(frameTime * MathF.Tau * 2f) * -3f;
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
            vel.Y += (_bellyOffset + _fakeWalkJiggle) * 0.6f;

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

    public bool IsSittingVisual()
    {
        if (Player.mount.Active)
        {
            switch (Player.mount.Type)
            {
                case MountID.GolfCartSomebodySaveMe:
                case MountID.WitchBroom:
                case MountID.SpookyWood:
                    return true;
                default:
                    return false;
            }
        }
        return (Player.sitting.isSitting && !Player.GetModPlayer<TreadmillPlayer>()._onTreadmill) || _finalMovementFactor < 0.01f;
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
        if (_physicsLayers != null)
        {
            foreach (WgPhysics.Layer layer in _physicsLayers)
                layer.DrawDataOverrides.Clear();
        }
        if (Player.isDisplayDollOrInanimate)
            drawInfo.Position.Y += Player.gfxOffY;
    }

    static void DrawHeldProj(PlayerDrawSet drawinfo, Projectile proj)
    {
        if (!ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[proj.type])
            proj.gfxOffY = drawinfo.drawPlayer.gfxOffY;
        try
        {
            Main.instance.DrawProjDirect(proj);
        }
        catch
        {
            proj.active = false;
        }
    }

    static void RenderAllLayers(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
    {
        if (!drawinfo.drawPlayer.TryGetModPlayer(out WgPlayer wg) || wg._physicsLayers == null)
        {
            orig(ref drawinfo);
            return;
        }
        List<DrawData> drawDataCache = drawinfo.DrawDataCache;
        if (_spriteBuffer == null)
            _spriteBuffer = new SpriteDrawBuffer(Main.graphics.GraphicsDevice, 200);
        else
            _spriteBuffer.CheckGraphicsDevice(Main.graphics.GraphicsDevice);
        for (int i = 0; i < drawDataCache.Count; i++)
        {
            DrawData drawData = drawDataCache[i];
            if (wg._physicsLayers != null && wg._physicsLayers.Exists(phys => phys.DrawDataOverrides.Contains(i)))
                continue;
            if (drawData.texture != null)
                drawData.Draw(_spriteBuffer);
        }
        _spriteBuffer.UploadAndBind();
        DrawData cdd = default;
        int drawCount = 0;
        for (int i = 0; i <= drawDataCache.Count; i++)
        {
            if (drawinfo.projectileDrawPosition == i)
            {
                if (cdd.shader != 0)
                    Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                _spriteBuffer.Unbind();
                DrawHeldProj(drawinfo, Main.projectile[drawinfo.drawPlayer.heldProj]);
                _spriteBuffer.Bind();
            }
            if (i != drawDataCache.Count)
            {
                cdd = drawDataCache[i];
                if (!cdd.sourceRect.HasValue)
                    cdd.sourceRect = cdd.texture.Frame();
                PlayerDrawHelper.SetShaderForData(drawinfo.drawPlayer, drawinfo.cHead, ref cdd);
                if (wg._physicsLayers != null)
                {
                    WgPhysics.Layer layer = wg._physicsLayers.Find(phys => phys.DrawDataOverrides.Contains(i));
                    if (layer != null)
                    {
                        _spriteBuffer.Unbind();
                        layer.Draw(Main.graphics.GraphicsDevice, cdd.texture, cdd.sourceRect.Value, cdd.color);
                        _spriteBuffer.Bind();
                        continue;
                    }
                }
                if (cdd.texture != null)
                    _spriteBuffer.DrawSingle(drawCount++);
            }
        }
        _spriteBuffer.Unbind();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
}
