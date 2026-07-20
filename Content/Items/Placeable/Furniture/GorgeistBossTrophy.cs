using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Tiles.Furniture;

namespace WgMod.Content.Items.Placeable.Furniture;

public class GorgeistBossTrophy : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<GorgeistBossTrophyTile>());

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(0, 1);
	}
}
