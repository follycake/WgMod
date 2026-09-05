using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgArmor
{
    public struct Layer
    {
        public Asset<Texture2D> BodyTexture;
        public Color BodyColor;

        public Asset<Texture2D> LegsTexture;
        public Asset<Texture2D> LegsGlowTexture;
        public Color LegsColor;

        public void SetBody(Asset<Texture2D> texture, Color color)
        {
            BodyTexture = texture;
            BodyColor = color;
        }

        public void SetLegs(Asset<Texture2D> texture, Color color)
        {
            LegsTexture = texture;
            LegsColor = color;
        }
    };

    public static bool Enabled => !WgClientConfig.Instance.DisableUVClothes && SpriteSet.Current.UVArmor;

    static List<DrawData> _drawData;
    static List<int> _dust;
    static List<int> _gore;
    static BlendState _multiplyBlend;

    public static void Render(WgPlayer wg)
    {
        if (!Shaders.FatArmor.IsLoaded || !Shaders.FatArmorLegs.IsLoaded)
            return;

        ref RenderTarget2D target = ref wg._armorTarget;
        PlayerDrawSet drawInfo = CreateDrawInfo(wg.Player);
        Span<Layer> layers = wg._armorLayers;
        SetupArmorLayers(drawInfo, layers);

        SpriteSet set = SpriteSet.GetSet(wg.Weight.GetStage());
        GraphicsDevice device = Main.graphics.GraphicsDevice;
        SpriteBatch spriteBatch = Main.spriteBatch;
        if (target == null || target.Width != set.ArmorAltasWidth || target.Height != set.ArmorAltasHeight)
        {
            target?.Dispose();
            target = new RenderTarget2D(device, set.ArmorAltasWidth, set.ArmorAltasHeight, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        device.SetRenderTarget(target);
        device.Clear(Color.Transparent);

        // Torso pass
        spriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            Shaders.FatArmor.Value
        );
        Vector2 baseOffset = wg.Player.Male ? new Vector2(0f, -0.5f) : Vector2.Zero;
        foreach (Layer layer in layers)
        {
            if (layer.BodyTexture == null)
                continue;
            device.Textures[1] = layer.BodyTexture.Value;
            Shaders.FatArmor.Value.Parameters["uImageSize1"].SetValue(layer.BodyTexture.Size());
            foreach (SpriteSet.Layer spriteLayer in set.ArmorLayers)
            {
                if (spriteLayer.LegArmor)
                    continue;
                if (spriteLayer.Type == SpriteSet.LayerType.Arms)
                {
                    Shaders.FatArmor.Value.Parameters["uOffset"].SetValue(Vector2.Zero);
                    Shaders.FatArmor.Value.Parameters["uGlowColor"].SetValue(drawInfo.armGlowColor.ToVector4());
                }
                else
                {
                    Shaders.FatArmor.Value.Parameters["uOffset"].SetValue(baseOffset);
                    Shaders.FatArmor.Value.Parameters["uGlowColor"].SetValue(drawInfo.bodyGlowColor.ToVector4());
                }
                spriteBatch.Draw(spriteLayer.ArmorTexture, new Vector2(spriteLayer.ArmorAtlasX, 0f), layer.BodyColor);
            }
        }
        spriteBatch.End();

        // Legs pass
        spriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            Shaders.FatArmorLegs.Value
        );
        Shaders.FatArmorLegs.Value.Parameters["uGlowColor"].SetValue(drawInfo.legsGlowColor.ToVector4());
        foreach (Layer layer in layers)
        {
            if (layer.LegsTexture == null)
                continue;
            device.Textures[1] = layer.LegsTexture.Value;
            device.Textures[2] = layer.LegsGlowTexture?.Value;
            Shaders.FatArmorLegs.Value.Parameters["uImageSize1"].SetValue(layer.LegsTexture.Size());
            Shaders.FatArmorLegs.Value.Parameters["uGlow"].SetValue(layer.LegsGlowTexture != null);
            foreach (SpriteSet.Layer spriteLayer in set.ArmorLayers)
            {
                if (!spriteLayer.LegArmor)
                    continue;
                spriteBatch.Draw(spriteLayer.ArmorTexture, new Vector2(spriteLayer.ArmorAtlasX, 0f), layer.LegsColor);
            }
        }
        spriteBatch.End();

        // Soften pass
        _multiplyBlend ??= new BlendState
        {
            AlphaBlendFunction = BlendFunction.Add,
            AlphaSourceBlend = Blend.Zero,
            AlphaDestinationBlend = Blend.One,

            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero
        };
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            _multiplyBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            Shaders.FatArmorSoften.Value
        );
        foreach (SpriteSet.Layer layer in set.ArmorLayers)
            spriteBatch.Draw(layer.Texture.Value, new Vector2(layer.ArmorAtlasX, 0f), Color.White);
        spriteBatch.End();

        device.SetRenderTarget(null);
    }

    public static bool ShouldDraw(in PlayerDrawSet drawInfo)
    {
        if (drawInfo.shadow != 0f && drawInfo.drawPlayer.body <= 0)
            return false;
        return Enabled;
    }

    public static void Draw(WgPlayer wg, ref PlayerDrawSet drawInfo, in DrawData baseDrawData, SpriteSet.Layer layer)
    {
        Rectangle rect = baseDrawData.sourceRect.Value;
        rect.X += layer.ArmorAtlasX;
        drawInfo.DrawDataCache.Add(baseDrawData with
        {
            texture = wg._armorTarget,
            sourceRect = rect,
            shader = layer.LegArmor ? drawInfo.drawPlayer.legs > 0 ? drawInfo.cLegs : 0 : drawInfo.drawPlayer.body > 0 ? drawInfo.cBody : 0,
            color = Color.Multiply(Color.White, 1f - drawInfo.shadow)
        });
    }

    public static Vector2 GetDrawPosition(Player player)
    {
        if (Main.gameMenu)
            return player.position;

        bool isSitting = player.sitting.isSitting;
        bool isSleeping = player.sleeping.isSleeping;

        if (player.mount.Active && player.mount.Type == MountID.GolfCartSomebodySaveMe)
            isSitting = true;
        if (player.mount.Active && player.mount.Type == MountID.WitchBroom)
            isSitting = true;
        if (player.mount.Active && player.mount.Type == MountID.SpookyWood)
            isSitting = true;

        Vector2 position = player.VisualPosition;
        position.X += player.MountXOffset * player.direction;
        if (isSitting)
        {
            player.sitting.GetSittingOffsetInfo(player, out Vector2 posOffset, out _);
            position += posOffset;
        }
        if (isSleeping)
        {
            player.sleeping.GetSleepingOffsetInfo(player, out Vector2 posOffset);
            position += posOffset;
        }
        position.Y -= player.HeightOffsetVisual;
        return position;
    }

    public static PlayerDrawSet CreateDrawInfo(Player player)
    {
        _drawData ??= [];
        _drawData.Clear();
        _dust ??= [];
        _dust.Clear();
        _gore ??= [];
        _gore.Clear();
        PlayerDrawSet drawInfo = new();
        drawInfo.BoringSetup(player, _drawData, _dust, _gore, player.VisualPosition, 0f, player.fullRotation, player.fullRotationOrigin);
        foreach (int dust in drawInfo.DustCache)
            Main.dust[dust].active = false;
        drawInfo.DustCache.Clear();
        foreach (int gore in drawInfo.GoreCache)
            Main.gore[gore].active = false;
        drawInfo.GoreCache.Clear();
        return drawInfo;
    }

    public static void SetupArmorLayers(in PlayerDrawSet drawInfo, Span<Layer> layers)
    {
        Player player = drawInfo.drawPlayer;
        layers.Clear();

        if (player.isDisplayDollOrInanimate)
            layers[0].SetBody(TextureAssets.Players[drawInfo.skinVar, 3], drawInfo.colorBodySkin);

        // Torso
        if (player.body > 0)
            layers[1].SetBody(TextureAssets.ArmorBodyComposite[player.body], drawInfo.colorArmorBody);
        else if (!player.isDisplayDollOrInanimate)
        {
            layers[0].SetBody(TextureAssets.Players[drawInfo.skinVar, 4], drawInfo.colorUnderShirt);
            layers[1].SetBody(TextureAssets.Players[drawInfo.skinVar, 8], drawInfo.colorUnderShirt);
            layers[2].SetBody(TextureAssets.Players[drawInfo.skinVar, 13], drawInfo.colorShirt);
            layers[3].SetBody(TextureAssets.Players[drawInfo.skinVar, 6], drawInfo.colorShirt);
        }

        // Legs
        if (player.legs > 0)
        {
            if (drawInfo.legsGlowMask >= 0)
                layers[1].LegsGlowTexture = TextureAssets.GlowMask[drawInfo.legsGlowMask];
            layers[1].SetLegs(TextureAssets.ArmorLeg[player.legs], drawInfo.colorArmorLegs);
        }
        else if (!player.isDisplayDollOrInanimate)
        {
            layers[1].SetLegs(TextureAssets.Players[drawInfo.skinVar, 11], drawInfo.colorPants);
            layers[2].SetLegs(TextureAssets.Players[drawInfo.skinVar, 12], drawInfo.colorShoes);
        }
        if (player.shoe > 0 && !(player.legs > 0 && ArmorIDs.Legs.Sets.OverridesLegs[player.legs]))
            layers[3].SetLegs(TextureAssets.AccShoes[player.shoe], drawInfo.colorShoes);
    }
}
