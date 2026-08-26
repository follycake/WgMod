using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn.Interactible;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class BarnBed : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.Interactible.BarnBed>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 2000;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
