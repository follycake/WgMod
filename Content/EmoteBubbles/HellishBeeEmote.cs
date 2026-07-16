using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace WgMod.Content.EmoteBubbles;

public class HellishBeeEmote : ModEmoteBubble
{
    public override void SetStaticDefaults()
    {
        AddToCategory(EmoteID.Category.CrittersAndMonsters);
    }
}
