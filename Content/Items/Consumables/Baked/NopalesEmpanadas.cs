using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Consumables.Baked;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.robinn9283)]
public class NopalesEmpanadas : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.FoodParticleColors[Type] =
        [
            new Color(69, 69, 69),
            new Color(69, 69, 69),
            new Color(69, 69, 69),
        ];

        ItemID.Sets.IsFood[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToFood(
            42,
            26,
            ModContent.BuffType<Buffs.Consumables.SpikedSkin>(),
            8 * 60 * 60
        );
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Cactus, 12)
            .AddIngredient(ItemID.SpicyPepper, 1)
            .AddTile<Tiles.Furniture.Oven>()
            .Register();
    }
}
