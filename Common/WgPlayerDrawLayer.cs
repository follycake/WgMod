using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgPlayerDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override void Load()
    {
        On_LegacyPlayerRenderer.DrawPlayerStoned += DrawPlayerStoned;
    }

    public override void Unload()
    {
        On_LegacyPlayerRenderer.DrawPlayerStoned -= DrawPlayerStoned;
    }

    public override Position GetDefaultPosition() => new Multiple()
    {
        { new Between(PlayerDrawLayers.Torso, PlayerDrawLayers.OffhandAcc), drawInfo => !CheckTop(drawInfo) },
        { new Between(PlayerDrawLayers.Head, PlayerDrawLayers.MountFront), CheckTop }
    };

    static bool CheckTop(PlayerDrawSet drawInfo)
    {
        if (Main.dedServ || !drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return false;
        return SpriteSet.GetStage(wg.Weight.GetStage()).OnTop;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    public static Color GetSkinColor(in PlayerDrawSet drawInfo)
    {
        if (drawInfo.headOnlyRender)
            return drawInfo.drawPlayer.skinColor;
        Color skinColor = drawInfo.colorBodySkin;
        if (drawInfo.drawPlayer.isDisplayDollOrInanimate)
            skinColor = new Color(154, 115, 85).MultiplyRGB(skinColor);
        return skinColor;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Draw(ref drawInfo, false);
        Draw(ref drawInfo, true);
    }

    public static void Draw(ref PlayerDrawSet drawInfo, bool top)
    {
        if (drawInfo.ShouldHidePlayer())
            return;
        Player player = drawInfo.drawPlayer;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        if (stage <= 0)
            return;

        SpriteSet.Stage stageData = SpriteSet.GetStage(stage, out SpriteSet set);
        SpriteSet.Layer[] layers = top ? set.TopLayers : set.Layers;
        if (layers.Length <= 0)
            return;

        int direction = ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) == 0).ToDirectionInt();
        Vector2 position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
        position += SpriteSet.GetOffset(set, stageData, direction, player.gravDir);

        if (Main.gameMenu)
            wg.UpdateAnimation();

        Color skinColor = GetSkinColor(drawInfo);
        float t = wg.Weight.ClampedImmobility;
        float bellySquish = float.Lerp(wg._squishPos, 1f, t * t * 0.2f);
        float baseSquish = (bellySquish + 1f) * 0.5f;

        bool drawArmor = WgArmor.ShouldDraw(drawInfo);
        foreach (SpriteSet.Layer layer in layers)
        {
            if (!layer.ShouldRender(player))
                continue;
            layer.Animate(wg, position, bellySquish, baseSquish, out Vector2 pos, out Vector2 scale);
            Rectangle layerFrame = layer.Frame(set, stageData);
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
            if (layer.Physics && WgPhysics.IsEnabled(wg))
            {
                WgPhysics.Layer phys = wg._physicsLayers[layer.PhysicsIndex];
                if (phys.Active)
                {
                    wg._physicsDrawOverride.Add(drawInfo.DrawDataCache.Count, phys);
                    if (drawArmor && layer.UVArmor)
                        wg._physicsDrawOverride.Add(drawInfo.DrawDataCache.Count + 1, phys);
                }
            }
            drawInfo.DrawDataCache.Add(drawData);
            if (drawArmor && layer.UVArmor)
                WgArmor.Draw(wg, ref drawInfo, drawData, layer);
        }
    }

    static void DrawPlayerStoned(On_LegacyPlayerRenderer.orig_DrawPlayerStoned orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer, Vector2 position)
    {
        orig(self, camera, drawPlayer, position);
        if (drawPlayer.dead || !drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        if (stage <= 0)
            return;

        SpriteSet.Stage stageData = SpriteSet.GetStage(stage, out SpriteSet set);
        SpriteEffects effects = drawPlayer.direction != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Vector2 drawPos = new Vector2((int)(position.X - camera.UnscaledPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(position.Y - camera.UnscaledPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 8f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
        Color drawColor = Lighting.GetColor((int)(position.X + drawPlayer.width * 0.5) / 16, (int)(position.Y + drawPlayer.height * 0.5) / 16, Color.White);

        int direction = drawPlayer.direction;
        Vector2 layerPos = drawPos;
        layerPos += SpriteSet.GetOffset(set, stageData, direction, drawPlayer.gravDir);
        layerPos.Y -= 8f;

        void DrawLayers(SpriteSet.Layer[] layers)
        {
            foreach (SpriteSet.Layer layer in layers)
            {
                Rectangle layerFrame = layer.Texture.Frame(1, set.FrameCount, 0, stageData.Frame);
                camera.SpriteBatch.Draw(layer.Texture.Value, layerPos, layerFrame, drawColor, 0f, layerFrame.Size() * 0.5f, 1f, effects, 0f);
            }
        }

        Shaders.ApplyStone(camera);
        DrawLayers(set.Layers);

        int armStage = stageData.Arm;
        Texture2D texture = armStage >= 0 ? set.ArmLayers[armStage].Texture.Value : TextureAssets.Players[drawPlayer.skinVariant, 3].Value;
        Rectangle frame = texture.Frame(9, 4, 2, 0);
        camera.SpriteBatch.Draw(texture, drawPos + new Vector2(0f, -4f), frame, drawColor, 0f, frame.Size() * 0.5f, 1f, effects, 0f);

        DrawLayers(set.TopLayers);
    }
}
