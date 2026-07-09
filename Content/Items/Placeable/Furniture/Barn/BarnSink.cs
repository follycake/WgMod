using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnSink : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Barn.BarnSink>());
        Item.width = 24;
        Item.height = 30;
        Item.value = 3000;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Hay, 6)
            .AddIngredient(ItemID.WaterBucket)
            .AddTile<Tiles.Furniture.Barn.BarnWorktable>()
            .Register();
    }
}
