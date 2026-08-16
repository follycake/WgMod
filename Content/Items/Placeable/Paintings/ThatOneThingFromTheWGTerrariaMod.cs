using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Paintings;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
public class ThatOneThingFromTheWGTerrariaMod : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.ThatOneThingFromTheWGTerrariaMod>());

        Item.width = 32;
        Item.height = 32;
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
