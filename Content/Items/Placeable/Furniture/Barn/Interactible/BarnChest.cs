using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn.Interactible;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class BarnChest : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.Interactible.BarnChest>());
        Item.width = 26;
        Item.height = 22;
        Item.value = 500;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 8)
            .AddIngredient(ItemID.IronBar, 2)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.Hay, 8)
            .AddIngredient(ItemID.LeadBar, 2)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
