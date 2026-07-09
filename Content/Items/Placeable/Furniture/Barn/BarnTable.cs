using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnTable : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.BarnTable>());
        Item.width = 38;
        Item.height = 24;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 8)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
