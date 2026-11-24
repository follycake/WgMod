using System.Collections.Generic;
using Terraria.ModLoader;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Common;
public class ItemCategories
{
    public static readonly HashSet<int> _weightGainPotions =
    [
        ModContent.ItemType<LesserWeightGainPotion>(),
        ModContent.ItemType<WeightGainPotion>(),
        ModContent.ItemType<GreaterWeightGainPotion>(),
        ModContent.ItemType<SuperWeightGainPotion>(),
    ];
    public static readonly HashSet<int> _weightLossPotions =
    [
        ModContent.ItemType<LesserWeightLossPotion>(),
        ModContent.ItemType<WeightLossPotion>(),
        ModContent.ItemType<GreaterWeightLossPotion>(),
        ModContent.ItemType<SuperWeightLossPotion>(),
    ];
}
