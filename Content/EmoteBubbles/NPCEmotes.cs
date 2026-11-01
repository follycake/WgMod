using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace WgMod.Content.EmoteBubbles;

// https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/EmoteBubbles/NPCEmotes.cs
// This abstract class is used for town NPC emotes quick setup.
public abstract class ModTownEmote : ModEmoteBubble
{
    // Redirecting texture path.
    public override string Texture => "WgMod/Content/EmoteBubbles/NPCEmotes";

    public override void SetStaticDefaults()
    {
        // Add NPC emotes to "Town" category.
        AddToCategory(EmoteID.Category.Town);
    }

    /// <summary>
    /// Which row of the sprite sheet is this NPC emote in?
    /// This is used to help get the correct frame rectangle for different emotes.
    /// </summary>
    public abstract int Row { get; }

    // You should decide the frame rectangle yourself by these two methods.
    public override Rectangle? GetFrame()
    {
        return new Rectangle(EmoteBubble.frame * 34, 28 * Row, 34, 28);
    }

    // Do note that you should never use EmoteBubble instance as the GetFrame() method above
    // in "Emote Menu Methods" (methods with -InEmoteMenu suffix).
    // Because in that case the value of EmoteBubble is always null.
    public override Rectangle? GetFrameInEmoteMenu(int frame, int frameCounter)
    {
        return new Rectangle(frame * 34, 28 * Row, 34, 28);
    }
}

public class GroundedHarpyEmote : ModTownEmote
{
    public override int Row => 0;
}

public class MilkmaidEmote : ModTownEmote
{
    public override int Row => 1;
}
