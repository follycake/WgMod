using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Placeable;

namespace WgMod.Content.Items.Accessories.Informational;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class PortableScale : ModItem
{
    public override void SetDefaults()
    {
        Item.accessory = true;
        Item.width = 30;
        Item.height = 30;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 5);
    }

    public override void UpdateInventory(Player player)
    {
        player.Wg()._displayWeight = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.Wg()._displayWeight = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddRecipeGroup(RecipeGroupID.PressurePlate)
            .AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddIngredient(ItemID.Wire, 4)
            .AddTile(TileID.Anvils)
            .Register();

        CreateRecipe()
            .AddIngredient<Scale>()
            .AddIngredient(ItemID.Wire, 4)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
