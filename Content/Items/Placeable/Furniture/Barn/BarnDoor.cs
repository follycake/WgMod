using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Tiles.Furniture.Barn;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnDoor : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<BarnDoorClosed>());
        Item.width = 14;
        Item.height = 28;
        Item.value = 150;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 6)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
