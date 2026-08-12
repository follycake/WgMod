using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Ranged;
using WgMod.Content.Items.Ammo;
using WgMod.Content.Items.Consumables.Baked;
using WgMod.Content.Items.Consumables.Potions;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Common.Systems;

public class ChestLootSystem : ModSystem
{
    record struct LootEntry(int Type, int Chance, int MinAmount = 1, int MaxAmount = 1, bool Ignore = false);

    const int AccessoryChance = 6;
    const int BuffPotionChance = 4;
    const int TieredPotionChance = 3;

    readonly WeightedRandom<int> _lesserWeightPotions = new();
    readonly WeightedRandom<int> _weightPotions = new();
    readonly WeightedRandom<int> _skywareLoot = new();

    public override void PostSetupContent()
    {
        _lesserWeightPotions.Add(ModContent.ItemType<LesserWeightGainPotion>());
        _lesserWeightPotions.Add(ModContent.ItemType<LesserWeightLossPotion>());

        _weightPotions.Add(ModContent.ItemType<WeightGainPotion>());
        _weightPotions.Add(ModContent.ItemType<WeightLossPotion>());

        _skywareLoot.Add(ModContent.ItemType<MeteorCrush>());
        _skywareLoot.Add(ModContent.ItemType<HeliumTank>());
    }

    public override void PostWorldGen()
    {
        for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            if (chest == null)
                continue;

            Tile tile = Main.tile[chest.x, chest.y];
            switch (tile.TileFrameX)
            {
                // Ice chest
                case 11 * 36:
                    //FillChest(chest, [new(ModContent.ItemType<AmuletOfStarving>(), AccessoryChance)]);
                    break;

                // Skyware chest
                case 13 * 36:
                    FillChest(chest, [
                        new(_skywareLoot, AccessoryChance)
                    ]);
                    break;

                // Mushroom chest
                case 32 * 36:
                    FillChest(chest, [new(ModContent.ItemType<StuffedTruffle>(), AccessoryChance)]);
                    break;

                // Living wood chest
                case 12 * 36:
                    FillChest(chest, [new(ModContent.ItemType<AcornCake>(), BuffPotionChance, 1, 2)]);
                    break;

                // Lihzahrd chest
                case 16 * 36:
                    FillChest(chest, [
                        new(ModContent.ItemType<WeightlessPotion>(), BuffPotionChance, 1, 2),
                        new(_weightPotions, TieredPotionChance, 3, 5),
                    ]);
                    break;

                // Gold chest
                case 1 * 36:
                    FillChest(chest, [new(ModContent.ItemType<CaramelArrow>(), 3, 25, 50)]);
                    break;
            }

            // Not lihzahrd chest
            if (tile.TileFrameX != 16 * 36)
            {
                FillChest(chest, [
                    new(ModContent.ItemType<WeightlessPotion>(), BuffPotionChance, 1, 2),
                    new(_lesserWeightPotions, TieredPotionChance, 3, 5),
                ]);
            }
        }
    }

    static void FillChest(Chest chest, Span<LootEntry> loot)
    {
        int lootIndex = 0;
        for (int i = 0; i < Chest.maxItems; i++)
        {
            Item item = chest.item[i];
            if (item.type != ItemID.None)
                continue;
            while (lootIndex < loot.Length)
            {
                LootEntry entry = loot[lootIndex];
                if (!Main.rand.NextBool(entry.Chance))
                {
                    lootIndex++;
                    continue;
                }
                int amount = Main.rand.Next(entry.MinAmount, entry.MaxAmount + 1);
                if (amount <= 0)
                {
                    lootIndex++;
                    continue;
                }
                item.SetDefaults(entry.Type);
                item.stack = amount;
                lootIndex++;
                break;
            }
        }
    }
}
