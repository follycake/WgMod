using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace WgMod.Content.EmoteBubbles;

public abstract class ModItemEmote : ModEmoteBubble
{
    public override string Texture => "WgMod/Content/EmoteBubbles/ItemEmotes";

    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.Items);
    }

    public abstract int Row { get; }

    public override Rectangle? GetFrame()
    {
        return new Rectangle(EmoteBubble.frame * 34, 28 * Row, 34, 28);
    }

    public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter)
    {
        return new Rectangle(frame * 34, 28 * Row, 34, 28);
    }
}

public class WeightLossEmote : ModItemEmote
{
    public override int Row => 0;
}

public class WeightGainEmote : ModItemEmote
{
    public override int Row => 1;
}
