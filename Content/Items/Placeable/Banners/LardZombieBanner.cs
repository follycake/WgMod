using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Tiles.Banners;

namespace WgMod.Content.Items.Placeable.Banners;

public class LardZombieBanner : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.KillsToBanner[Type] = 20;
    }
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanner>(), (int)EnemyBanner.StyleID.LardZombie);
        Item.width = 10;
        Item.height = 24;
        Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }
}
