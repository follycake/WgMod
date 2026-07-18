using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgArmor
{
    public record struct Layer(Asset<Texture2D> ArmorTexture, Color Color);

    public static bool Enabled => !WgClientConfig.Instance.DisableUVClothes && SpriteSet.Current.UVArmor;
    public static Asset<Effect> UVShader { get; private set; }
    public static Asset<Effect> SoftenShader { get; private set; }
    static BlendState _multiplyBlend;

    public static void Load(Mod mod)
    {
        UVShader = mod.Assets.Request<Effect>("Assets/Effects/FatArmor");
        SoftenShader = mod.Assets.Request<Effect>("Assets/Effects/FatArmorSoften");
    }

    public static void Render(ref RenderTarget2D target, ReadOnlySpan<Layer> layers, bool male)
    {
        if (!UVShader.IsLoaded)
            return;

        SpriteSet set = SpriteSet.Current;
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
            UVShader.Value
        );
        Vector2 baseOffset = male ? new Vector2(0f, -0.5f) : Vector2.Zero;
        foreach (Layer layer in layers)
        {
            if (layer.ArmorTexture == null)
                continue;

            device.Textures[1] = layer.ArmorTexture.Value;
            UVShader.Value.Parameters["uImageSize1"].SetValue(layer.ArmorTexture.Size());

            foreach (SpriteSet.Layer spriteLayer in set.ArmorLayers)
            {
                UVShader.Value.Parameters["uOffset"].SetValue(spriteLayer.Type == SpriteSet.LayerType.Arms ? Vector2.Zero : baseOffset);
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
            SoftenShader.Value
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
            color = drawInfo.drawPlayer.GetImmuneAlphaPure(Color.White, drawInfo.shadow)
        });
    }

    public static void ConvertSimple(Texture2D texture)
    {
        if (texture.Format != SurfaceFormat.Color)
            throw new Exception("Invalid texture format.");
        Color[] colors = new Color[texture.Width * texture.Height];
        texture.GetData(colors);
        for (int i = 0; i < colors.Length; i++)
        {
            if (_torsoColors.TryGetValue(colors[i], out Color uv))
                colors[i] = uv;
            else
                colors[i] = Color.Transparent;
        }
        texture.SetData(colors);
    }

    static readonly Dictionary<Color, Color> _torsoColors = new()
    {
        // Red
        [new(127, 0, 0)] = new(13, 167, 0),
        [new(255, 0, 0)] = new(15, 167, 0),
        [new(255, 127, 127)] = new(16, 167, 0),

        // Yellow
        [new(127, 127, 0)] = new(19, 167, 0),
        [new(255, 255, 0)] = new(18, 167, 0),

        // Cyan
        [new(0, 127, 127)] = new(19, 165, 0),
        [new(0, 255, 255)] = new(18, 165, 0),

        // Magenta
        [new(127, 0, 127)] = new(13, 165, 0),
        [new(255, 0, 255)] = new(15, 165, 0),
        [new(255, 127, 255)] = new(16, 165, 0),

        // Green
        [new(0, 127, 0)] = new(18, 172, 0),
        [new(0, 255, 0)] = new(16, 172, 0),

        // Blue
        [new(0, 0, 127)] = new(12, 172, 0),
        [new(0, 0, 255)] = new(13, 172, 0),
        [new(127, 127, 255)] = new(15, 172, 0),

        // White
        [new(127, 127, 127)] = new(12, 174, 0),
        [new(255, 255, 255)] = new(13, 174, 0)
    };
}
