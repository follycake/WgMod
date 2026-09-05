using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Consumables.Baked;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class SlimePie : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.FoodParticleColors[Type] =
        [
            new Color(0, 161, 213),
            new Color(127, 138, 255),
            new Color(194, 201, 129)
        ];

        ItemID.Sets.IsFood[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToFood(
            22,
            22,
            ModContent.BuffType<Buffs.Consumables.WobWobWobWob>(),
            8 * 60 * 60
        );
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SlimeBlock, 6)
            .AddIngredient(ItemID.Gel, 12)
            .AddIngredient(ItemID.PinkGel, 3)
            .AddTile<Tiles.Furniture.Oven>()
            .Register();
    }
}
