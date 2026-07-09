using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnClock : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.BarnClock>());
        Item.width = 26;
        Item.height = 22;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 10)
            .AddIngredient(ItemID.IronBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.Hay, 10)
            .AddIngredient(ItemID.LeadBar, 3)
            .AddIngredient(ItemID.Glass, 6)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
