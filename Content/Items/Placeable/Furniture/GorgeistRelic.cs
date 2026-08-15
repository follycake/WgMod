using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class GorgeistRelic : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.GorgeistRelic>(), 0);

		Item.width = 30;
		Item.height = 40;
		Item.rare = ItemRarityID.Master;
		Item.master = true;
		Item.value = Item.buyPrice(0, 5);
	}
}
