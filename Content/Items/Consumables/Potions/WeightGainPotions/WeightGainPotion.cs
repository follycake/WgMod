using Terraria;
using Terraria.ID;

namespace WgMod.Content.Items.Consumables.Potions.WeightGainPotions;

public class LesserWeightGainPotion : WeightPotion
{
    public override float WeightEffect => 10f;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 20;
        Item.height = 24;
        Item.rare = ItemRarityID.White;
        Item.value = Item.buyPrice(silver: 3);
    }
}

public class WeightGainPotion : WeightPotion
{
    public override float WeightEffect => 20f;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 30;
        Item.height = 24;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(silver: 6);
    }
}

public class GreaterWeightGainPotion : WeightPotion
{
    public override float WeightEffect => 30f;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 32;
        Item.height = 32;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(silver: 10);
    }
}

public class SuperWeightGainPotion : WeightPotion
{
    public override float WeightEffect => 40f;

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 64;
        Item.height = 58;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.buyPrice(silver: 30);
    }
}
