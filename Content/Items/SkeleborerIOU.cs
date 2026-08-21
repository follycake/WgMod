using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Items;

public class SkeleborerIOU : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 58;
        Item.height = 28;

        Item.maxStack = Item.CommonMaxStack;
    }
}
