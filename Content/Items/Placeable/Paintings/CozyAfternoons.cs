using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Paintings;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class CozyAfternoons : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Paintings.CozyAfternoons>());

        Item.width = 48;
        Item.height = 32;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Amethyst, 8)
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}
