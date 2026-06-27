using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class Treadmill : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Treadmill>());
        Item.width = 28;
        Item.height = 20;
        Item.value = 20000;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.AsphaltBlock, 8)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
