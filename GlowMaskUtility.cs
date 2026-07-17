using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace WgMod;

public static class GlowMaskUtility
{
    /// <summary>
    /// Returns an id that you can use for glow masks
    /// </summary>
    public static int AddGlowMask(string texture)
    {
        return AddGlowMask(ModContent.Request<Texture2D>(texture));
    }

    /// <summary>
    /// Returns an id that you can use for glow masks
    /// </summary>
    public static int AddGlowMask(Asset<Texture2D> texture)
    {
        int id = TextureAssets.GlowMask.Length;
        Array.Resize(ref TextureAssets.GlowMask, id + 1);
        TextureAssets.GlowMask[id] = texture;
        return id;
    }
}
