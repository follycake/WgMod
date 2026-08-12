using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgArmor
{
    public record struct Layer(Asset<Texture2D> ArmorTexture, Color Color);

    public static bool Enabled => !WgClientConfig.Instance.DisableUVClothes && SpriteSet.Current.UVArmor;
    static BlendState _multiplyBlend;

    public static void Render(int stage, ref RenderTarget2D target, ReadOnlySpan<Layer> layers, bool male)
    {
        if (!Shaders.FatArmor.IsLoaded)
            return;

        SpriteSet set = SpriteSet.GetSet(stage);
        GraphicsDevice device = Main.graphics.GraphicsDevice;
        SpriteBatch spriteBatch = Main.spriteBatch;
        target ??= new RenderTarget2D(device, set.ArmorAltasWidth, set.ArmorAltasHeight, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);

        device.SetRenderTarget(target);
        device.Clear(Color.Transparent);

        spriteBatch.Begin(
            SpriteSortMode.Immediate,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullCounterClockwise,
            Shaders.FatArmor.Value
        );
        Vector2 baseOffset = male ? new Vector2(0f, -0.5f) : Vector2.Zero;
        foreach (Layer layer in layers)
        {
            if (layer.ArmorTexture == null)
                continue;

            device.Textures[1] = layer.ArmorTexture.Value;
            Shaders.FatArmor.Value.Parameters["uImageSize1"].SetValue(layer.ArmorTexture.Size());

            foreach (SpriteSet.Layer spriteLayer in set.ArmorLayers)
            {
                Shaders.FatArmor.Value.Parameters["uOffset"].SetValue(spriteLayer.Type == SpriteSet.LayerType.Arms ? Vector2.Zero : baseOffset);
                spriteBatch.Draw(spriteLayer.ArmorTexture, new Vector2(spriteLayer.ArmorAtlasX, 0f), layer.Color);
            }
        }
        spriteBatch.End();

        _multiplyBlend ??= new BlendState()
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
            shader = drawInfo.drawPlayer.body > 0 ? drawInfo.cBody : 0,
            // Vanilla uses GetImmuneAlpha for body texture, using GetImmuneAlphaPure puts body and armor out of sync
            color = drawInfo.drawPlayer.GetImmuneAlpha(Color.White, drawInfo.shadow)
        });
    }

    // Hurt effect is already applied in drawInfo, so bake lighting only
    static Color Light(Player player, Vector2 position, Color color)
    {
        return Lighting.GetColorClamped((int)(position.X + player.width * 0.5) / 16, (int)((position.Y + player.height * 0.5) / 16.0), color);
    }

    static Vector2 GetDrawPosition(Player player)
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

        Vector2 position = player.position;
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

    public static void SetupArmorLayers(WgPlayer wg)
    {
        Player player = wg.Player;
        Vector2 position = GetDrawPosition(player);

        Array.Clear(wg._armorLayers);
        if (player.body > 0)
        {
            Color lit = Light(player, position, Color.White);
            if (player.isDisplayDollOrInanimate)
                wg._armorLayers[0] = new(TextureAssets.Players[player.skinVariant, 3], lit);
            wg._armorLayers[1] = new(TextureAssets.ArmorBodyComposite[player.body], lit);
        }
        else
        {
            if (player.isDisplayDollOrInanimate)
                wg._armorLayers[0] = new(TextureAssets.Players[player.skinVariant, 3], Light(player, position, Color.White));
            else
            {
                Color underShirt = Light(player, position, player.underShirtColor);
                Color shirt = Light(player, position, player.shirtColor);
                wg._armorLayers[0] = new(TextureAssets.Players[player.skinVariant, 4], underShirt);
                wg._armorLayers[1] = new(TextureAssets.Players[player.skinVariant, 8], underShirt);
                wg._armorLayers[2] = new(TextureAssets.Players[player.skinVariant, 13], shirt);
                wg._armorLayers[3] = new(TextureAssets.Players[player.skinVariant, 6], shirt);
            }
        }
    }
}
