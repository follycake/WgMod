using WgMod.Content.Tiles.Banners;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Banners;

public class SkeleborerBanner : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<EnemyBanner>());
		Item.width = 10;
		Item.height = 24;
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 10));
	}
}

