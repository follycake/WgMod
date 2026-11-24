using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class AltarOfCurses : ModItem
{
    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.LightPurple;
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AltarOfCurses>());
        Item.width = 32;
        Item.height = 32;
        Item.value = Item.sellPrice(silver: 2, copper: 30);
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.CraftingObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.EbonstoneBlock, 15)
            .AddIngredient(ItemID.RottenChunk, 4)
            .AddTile(TileID.DemonAltar)
            .Register();
        CreateRecipe()
            .AddIngredient(ItemID.CrimstoneBlock, 15)
            .AddIngredient(ItemID.Vertebrae, 4)
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}
