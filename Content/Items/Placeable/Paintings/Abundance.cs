using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Paintings;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
public class Abundance : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.Abundance>());

        Item.width = 20;
        Item.height = 30;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Topaz, 8)
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}
