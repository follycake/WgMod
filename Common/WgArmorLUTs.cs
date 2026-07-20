using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace WgMod.Common;

public static class WgArmorLUTs
{
    static Dictionary<Color, Color>[] _lookups;

    public static void Initialize(Mod mod)
    {
        int stride;
        Color[] colors;
        using (Texture2D texture = mod.Assets.Request<Texture2D>("Assets/Textures/ArmorLookup", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value)
        {
            stride = texture.Width;
            colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
        }
        _lookups = new Dictionary<Color, Color>[colors.Length / (stride * 2)];
        for (int y = 0; y < _lookups.Length; y++)
        {
            Dictionary<Color, Color> lookup = new(stride);
            for (int x = 0; x < stride; x++)
            {
                int i = x + y * stride * 2;
                lookup.Add(colors[i], colors[i + stride]);
            }
            _lookups[y] = lookup;
        }
    }

    public static bool TryGetUV(int lookup, Color color, out Color uv)
    {
        return _lookups[lookup].TryGetValue(color, out uv);
    }

    public static void ConvertSimple(int lookup, Texture2D texture)
    {
        if (texture.Format != SurfaceFormat.Color)
            throw new Exception("Invalid texture format.");
        Color[] colors = new Color[texture.Width * texture.Height];
        texture.GetData(colors);
        for (int i = 0; i < colors.Length; i++)
        {
            if (TryGetUV(lookup, colors[i], out Color uv))
                colors[i] = uv;
            else
                colors[i] = Color.Transparent;
        }
        texture.SetData(colors);
    }
}
