using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Consumables;

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

public class SkeleborerRewards : GlobalItem
{
    public override void AddRecipes()
    {
        Recipe.Create(ModContent.ItemType<GreedyFlask>())
            .AddIngredient(ModContent.ItemType<SkeleborerIOU>())
            .Register();
    }
}
