using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class FatteningDartTrap : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FatteningDartTrap>());

		Item.width = 12;
		Item.height = 12;
		Item.value = 10000;
		Item.mech = true; // lets you see wires while holding.
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.DartTrap)
			.AddIngredient(ItemID.BottledHoney)
			.Register();
		CreateRecipe()
			.AddIngredient(ItemID.DartTrap)
			.AddCondition(Condition.NearHoney)
			.Register();
	}
}
