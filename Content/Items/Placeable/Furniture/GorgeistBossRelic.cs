using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

public class GorgeistBossRelic : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.GorgeistBossRelicTile>(), 0);

		Item.width = 30;
		Item.height = 40;
		Item.rare = ItemRarityID.Master;
		Item.master = true;
		Item.value = Item.buyPrice(0, 5);
	}
}
