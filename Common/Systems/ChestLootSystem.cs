using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Ranged;
using WgMod.Content.Items.Ammo;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Consumables.Baked;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Common.Systems;

public class ChestLoot : ModSystem
{
    public override void PostWorldGen()
    {
        for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
        {
            Chest chest = Main.chest[chestIndex];
            int[] lesserWeightPotions = [ModContent.ItemType<LesserWeightGainPotion>(), ModContent.ItemType<LesserWeightLossPotion>()];
            int[] weightPotions = [ModContent.ItemType<WeightGainPotion>(), ModContent.ItemType<WeightLossPotion>()];
            int weightPotionAmount = Main.rand.Next(3, 6);
            int arrowAmount = Main.rand.Next(25, 51);
            int buffPotionAmount = Main.rand.Next(1, 3);

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 11 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AmuletOfStarving>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 13 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<MeteorCrush>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        else if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<HeliumTank>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 32 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<StuffedTruffle>());
                            chest.item[inventoryIndex].stack = 1;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 12 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<AcornCake>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX != 16 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<WeightlessPotion>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        else if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(lesserWeightPotions[Main.rand.Next(0, 2)]);
                            chest.item[inventoryIndex].stack = weightPotionAmount;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 16 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<WeightlessPotion>());
                            chest.item[inventoryIndex].stack = buffPotionAmount;
                        }
                        else if (Main.rand.NextBool(2))
                        {
                            chest.item[inventoryIndex].SetDefaults(weightPotions[Main.rand.Next(0, 2)]);
                            chest.item[inventoryIndex].stack = weightPotionAmount;
                        }
                        break;
                    }
                }
            }

            if (chest != null && Main.tile[chest.x, chest.y].TileFrameX == 1 * 36)
            {
                for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                {
                    if (chest.item[inventoryIndex].type == ItemID.None)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<CaramelArrow>());
                            chest.item[inventoryIndex].stack = arrowAmount;
                        }
                        break;
                    }
                }
            }
        }
    }
}
