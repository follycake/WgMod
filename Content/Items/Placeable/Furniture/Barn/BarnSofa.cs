using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class BarnSofa : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.BarnSofa>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 2000;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 5)
            .AddIngredient(ItemID.Silk, 2)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
