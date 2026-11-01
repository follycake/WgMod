using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Systems;
using WgMod.Content.Items.Consumables.Potions;

namespace WgMod.Common.GlobalItems;

public class LootGlobalItem : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        switch (item.type)
        {
            case ItemID.WoodenCrate:
            case ItemID.WoodenCrateHard:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<WeightlessPotion>(), ChestLootSystem.BuffPotionChance, 1, 2));
                itemLoot.Add(ItemDropRule.OneFromOptions(ChestLootSystem.TieredPotionChance, ChestLootSystem.LesserWeightPotions));
                break;
            case ItemID.FloatingIslandFishingCrate:
            case ItemID.FloatingIslandFishingCrateHard:
                itemLoot.Add(ItemDropRule.OneFromOptions(ChestLootSystem.AccessoryChance, ChestLootSystem.SkywareLoot));
                break;
        }
    }
}
