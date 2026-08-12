using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using static WgMod.Content.Projectiles.Melee.WeightPotionProjectile;

namespace WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

public class LesserWeightLossPotion : WeightPotion
{
    public override float WeightEffect => -10f;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<LesserWeightGainPotion>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 26;
        Item.rare = ItemRarityID.White;
        Item.value = Item.buyPrice(silver: 3);

        Item.shoot = ModContent.ProjectileType<LesserWeightLossPotionProjectile>();
    }
}

public class WeightLossPotion : WeightPotion
{
    public override float WeightEffect => -20f;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WeightGainPotion>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 16;
        Item.height = 24;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 6);

        Item.shoot = ModContent.ProjectileType<WeightLossPotionProjectile>();
    }
}

public class GreaterWeightLossPotion : WeightPotion
{
    public override float WeightEffect => -30f;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GreaterWeightGainPotion>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 18;
        Item.height = 30;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(silver: 10);

        Item.shoot = ModContent.ProjectileType<GreaterWeightLossPotionProjectile>();
    }
}

public class SuperWeightLossPotion : WeightPotion
{
    public override float WeightEffect => -40f;

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 30;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SuperWeightGainPotion>();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 22;
        Item.height = 32;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.buyPrice(silver: 30);

        Item.shoot = ModContent.ProjectileType<SuperWeightLossPotionProjectile>();
    }
}
