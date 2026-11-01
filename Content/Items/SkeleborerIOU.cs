using Terraria;
using Terraria.ID;
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

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Material;
    }
}
