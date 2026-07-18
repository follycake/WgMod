using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override void Load()
    {
        if (Main.dedServ)
            return;
        WgArmor.Load(Mod);
    }

    // folly: What is OffhandAcc exactly???
    public override Position GetDefaultPosition() => new Multiple()
    {
        { new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc), drawInfo => !CheckTop(drawInfo) },
        { new Between(PlayerDrawLayers.Head, PlayerDrawLayers.ArmOverItem), CheckTop }
    };

    static bool CheckTop(PlayerDrawSet drawInfo)
    {
        if (Main.dedServ || !drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return false;
        return SpriteSet.Current.GetStage(wg.Weight.GetStage()).OnTop;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    public static void SetupArmorLayers(WgPlayer wg)
    {
        PlayerDrawSet drawInfo = new()
        {
            drawPlayer = wg.Player,
            skinVar = wg.Player.skinVariant,
            colorArmorBody = Color.White,
            colorUnderShirt = wg.Player.underShirtColor,
            colorShirt = wg.Player.shirtColor
        };
        SetupArmorLayers(wg, drawInfo);
    }

    public static void SetupArmorLayers(WgPlayer wg, in PlayerDrawSet drawInfo)
    {
        Array.Clear(wg._armorLayers);
        if (drawInfo.drawPlayer.body > 0)
            wg._armorLayers[0] = new(TextureAssets.ArmorBodyComposite[drawInfo.drawPlayer.body], drawInfo.colorArmorBody);
        else
        {
            wg._armorLayers[0] = new(TextureAssets.Players[drawInfo.skinVar, 4], drawInfo.colorUnderShirt);
            wg._armorLayers[1] = new(TextureAssets.Players[drawInfo.skinVar, 8], drawInfo.colorUnderShirt);
            wg._armorLayers[2] = new(TextureAssets.Players[drawInfo.skinVar, 13], drawInfo.colorShirt);
            wg._armorLayers[3] = new(TextureAssets.Players[drawInfo.skinVar, 6], drawInfo.colorShirt);
        }
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        if (player.dead)
            return;

        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        int stage = wg.Weight.GetStage();
        if (stage <= 0)
            return;

        int direction = ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) == 0).ToDirectionInt();
        Vector2 position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
        position.X += WeightValues.DrawOffsetX(stage) * direction;

        float yOffset = 4f;
        Rectangle legFrame = player.legFrame;
        int frame = legFrame.Y / legFrame.Height;
        // Frame [0] - Idle
        // Frame [5] - Jump
        // Frame [6 to 19] - Walk

        float legOffsetX = 0f;
        float legOffsetY = 0f;
        float bellyOffset = 0f;
        if (wg._finalMovementFactor > 0.01f)
        {
            if (frame == 5)
                bellyOffset = Math.Clamp(player.velocity.Y * player.gravDir / 4f, -1f, 1f) * -2f;
            else if (frame >= 6 && frame <= 19)
            {
                float frameTime = (frame - 6) / 13f;
                legOffsetX = MathF.Sin(frameTime * MathF.Tau) * 2f * direction;
                legOffsetY = MathF.Max(MathF.Cos(frameTime * MathF.Tau), 0f) * -2f;
                bellyOffset = MathF.Sin(frameTime * MathF.Tau * 2f) * -2f;
            }
        }
        wg._bellyOffset = bellyOffset;

        Color skinColor = drawInfo.colorBodySkin; //player.GetImmuneAlphaPure(player.skinColor, drawInfo.shadow);
        float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, t * t * 0.4f);
        float baseSquish = (bellySquish + 1f) * 0.5f;

        SpriteSet set = SpriteSet.Current;
        bool drawArmor = WgArmor.ShouldDraw(drawInfo);
        if (drawArmor)
            SetupArmorLayers(wg, drawInfo);

        int stageFrame = set.GetStage(stage).Frame;
        foreach (SpriteSet.Layer layer in set.Layers)
        {
            if (!layer.ShouldRender(player))
                continue;
            Vector2 pos;
            Vector2 scale;
            switch (layer.Type)
            {
                case SpriteSet.LayerType.Legs:
                    pos = PrepPos(position, MathF.Round(legOffsetX / 2f) * 2f, yOffset + MathF.Round(legOffsetY / 2f) * 2f, player.gravDir);
                    scale = new Vector2(1f * baseSquish, 1f / baseSquish);
                    break;
                case SpriteSet.LayerType.Breasts:
                    pos = PrepPos(position, 0f, yOffset + MathF.Round(bellyOffset / 2f) * 2f, player.gravDir);
                    scale = new Vector2(1f * baseSquish, 1f / baseSquish);
                    break;
                default:
                    pos = PrepPos(position, 0f, yOffset + MathF.Round(bellyOffset / 2f) * 2f, player.gravDir);
                    scale = new Vector2(1f / bellySquish, 1f * bellySquish);
                    break;
            }
            Rectangle layerFrame = layer.Texture.Frame(1, set.FrameCount, 0, stageFrame);
            DrawData drawData = new(
                layer.Texture.Value, // The texture to render.
                pos, // Position to render at.
                layerFrame, // Source rectangle.
                skinColor, // Color.
                0f, // Rotation.
                layerFrame.Size() * 0.5f, // Origin. Uses the texture's center.
                scale, // Scale.
                drawInfo.playerEffect
            );
            drawInfo.DrawDataCache.Add(drawData);
            if (drawArmor && layer.UVArmor)
                WgArmor.Draw(wg, ref drawInfo, drawData, layer);
        }
    }

    static Vector2 PrepPos(Vector2 pos, float xOffset, float yOffset, float gravDir)
    {
        pos.X += xOffset;
        pos.Y += yOffset * gravDir;
        return pos.Floor();
    }
}
