using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class MarbleOven : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Oven>(), 0);
        Item.width = 36;
        Item.height = 26;
        Item.value = 500;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CookingPot)
            .AddIngredient(ItemID.Marble, 12)
            .AddIngredient(ItemID.Fireblossom, 6)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
