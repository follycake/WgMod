using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

public class GorgeistTrophy : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.GorgeistTrophy>());

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(0, 1);
	}
}
