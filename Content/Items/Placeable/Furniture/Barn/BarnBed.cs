using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnBed : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.BarnBed>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 2000;
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
