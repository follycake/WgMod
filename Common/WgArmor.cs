using System;
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
    static BlendState _multiplyBlend;

    public static void Render(int stage, ref RenderTarget2D target, ReadOnlySpan<Layer> layers, bool male)
    {
        if (!Shaders.FatArmor.IsLoaded || !Shaders.FatArmorLegs.IsLoaded)
            return;

        SpriteSet set = SpriteSet.GetSet(stage);
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
        Vector2 baseOffset = male ? new Vector2(0f, -0.5f) : Vector2.Zero;
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
                Shaders.FatArmor.Value.Parameters["uOffset"].SetValue(spriteLayer.Type == SpriteSet.LayerType.Arms ? Vector2.Zero : baseOffset);
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
            shader = layer.LegArmor ? (drawInfo.drawPlayer.legs > 0 ? drawInfo.cLegs : 0) : (drawInfo.drawPlayer.body > 0 ? drawInfo.cBody : 0),
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

    static int GetLegsGlowMask(Player drawPlayer)
    {
        var legsGlowMask = drawPlayer.legs switch
        {
            ArmorIDs.Legs.NebulaLeggings => GlowMaskID.NebulaArmorLegs,
            ArmorIDs.Legs.ArkhalisPants_Male => GlowMaskID.ArkhalisPants_Male,
            ArmorIDs.Legs.ArkhalisPants_Female => GlowMaskID.ArkhalisPants_Female,
            ArmorIDs.Legs.GroxTheGreatGreaves => GlowMaskID.GroxTheGreatGreaves,
            ArmorIDs.Legs.TimelessTravelerBottom => GlowMaskID.TimelessTravelerBottom,
            ArmorIDs.Legs.CapricornLegs => GlowMaskID.CapricornLegs,
            ArmorIDs.Legs.CapricornTail => GlowMaskID.CapricornTail,
            ArmorIDs.Legs.VortexLeggings => GlowMaskID.VortexArmorLegs,
            ArmorIDs.Legs.LokisGreaves => GlowMaskID.LokisLegs,
            ArmorIDs.Legs.StardustLeggings => GlowMaskID.ArmorStardustLegs,
            _ => -1
        };
        Color colorArmorLegs = Color.White;
        Color legsGlowColor = Color.Transparent;
        ItemLoader.DrawArmorColor(EquipType.Legs, drawPlayer.legs, drawPlayer, 0f, ref colorArmorLegs, ref legsGlowMask, ref legsGlowColor);
        return legsGlowMask;
    }

    public static void SetupArmorLayers(Player player, Layer[] layers)
    {
        Vector2 position = GetDrawPosition(player);
        Array.Clear(layers);

        Color lit = Light(player, position, Color.White);
        if (player.isDisplayDollOrInanimate)
            layers[0].SetBody(TextureAssets.Players[player.skinVariant, 3], lit);

        // Torso
        if (player.body > 0)
            layers[1].SetBody(TextureAssets.ArmorBodyComposite[player.body], lit);
        else if (!player.isDisplayDollOrInanimate)
        {
            Color underShirt = Light(player, position, player.underShirtColor);
            Color shirt = Light(player, position, player.shirtColor);
            layers[0].SetBody(TextureAssets.Players[player.skinVariant, 4], underShirt);
            layers[1].SetBody(TextureAssets.Players[player.skinVariant, 8], underShirt);
            layers[2].SetBody(TextureAssets.Players[player.skinVariant, 13], shirt);
            layers[3].SetBody(TextureAssets.Players[player.skinVariant, 6], shirt);
        }

        // Legs
        if (player.legs > 0)
        {
            int glowMask = GetLegsGlowMask(player);
            if (glowMask >= 0)
                layers[1].LegsGlowTexture = TextureAssets.GlowMask[glowMask];
            layers[1].SetLegs(TextureAssets.ArmorLeg[player.legs], lit);
        }
        else if (!player.isDisplayDollOrInanimate)
        {
            Color pants = Light(player, position, player.pantsColor);
            Color shoes = Light(player, position, player.shoeColor);
            layers[1].SetLegs(TextureAssets.Players[player.skinVariant, 11], pants);
            layers[2].SetLegs(TextureAssets.Players[player.skinVariant, 12], shoes);
        }
        if (player.shoe > 0 && !(player.legs > 0 && ArmorIDs.Legs.Sets.OverridesLegs[player.legs]))
            layers[3].SetLegs(TextureAssets.AccShoes[player.shoe], lit);
    }
}
