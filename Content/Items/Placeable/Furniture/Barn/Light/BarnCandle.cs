using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn.Light;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnCandle : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.Light.BarnCandle>());
        Item.width = 12;
        Item.height = 30;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 4)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
