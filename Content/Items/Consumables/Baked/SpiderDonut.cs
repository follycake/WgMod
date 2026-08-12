using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Consumables.Baked;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class SpiderDonut : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.FoodParticleColors[Type] =
        [
            new Color(89, 51, 40),
            new Color(176, 157, 202),
            new Color(236, 176, 88),
        ];

        ItemID.Sets.IsFood[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToFood(22, 22, ModContent.BuffType<Buffs.Consumables.FullOfSpider>(), 8 * 60 * 60);
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SpiderFang, 8)
            .AddTile<Tiles.Furniture.Oven>()
            .Register();
    }
}
