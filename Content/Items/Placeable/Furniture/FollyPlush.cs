using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class FollyPlush : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.FollyPlush>());
        Item.width = 14;
        Item.height = 15;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Silk, 10)
            .AddIngredient(ItemID.PinkThread, 3)
            .AddTile(TileID.Loom)
            .Register();
    }
}
