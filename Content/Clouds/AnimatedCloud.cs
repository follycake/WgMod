using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace WgMod.Content.Clouds;

public abstract class AnimatedCloud : ModCloud
{
    public abstract int FrameCount { get; }
    public abstract double FrameDuration { get; }

    public override bool Draw(SpriteBatch spriteBatch, Cloud cloud, int cloudIndex, ref DrawData drawData)
    {
        Texture2D texture = drawData.texture;
        drawData.position -= drawData.origin;
        drawData.sourceRect = texture.Frame(1, FrameCount, 0, (int)(Main.timeForVisualEffects / FrameDuration) % FrameCount);
        drawData.origin = drawData.sourceRect.Value.Size() * 0.5f;
        drawData.position += drawData.origin;
        return true;
    }
}
