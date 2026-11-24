using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common;
using WgMod.Content.Buffs.Debuffs;
using WgMod.Content.Items.Consumables;
using WgMod.Content.Items.Consumables.Potions;

namespace WgMod.Content.Items;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class CopperFunnel : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.maxStack = 1;
        Item.value = Item.sellPrice(silver: 4);

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 25;
        Item.useAnimation = 25;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override bool? UseItem(Player player)
    {
        if (player != Main.LocalPlayer)
            return null;

        Vector2 mouse = Main.MouseWorld;

        Item usedFood = GetFood(player);

        if (player.IsTargetTileInItemRange(Item) && usedFood != null) //a bit of a dumb way to get range but i guess it works
        {
            Player targetPlr = null;
            foreach (var plr in Main.ActivePlayers)
            {
                if (plr == player)
                    continue;
                if (plr.Hitbox.Contains(mouse.ToPoint()) && !plr.dead)
                {
                    targetPlr = plr;
                    break;
                }
            }
            if (targetPlr != null)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)WgMod.MessageType.FeederTool);
                    packet.Write((byte)player.whoAmI);
                    packet.Write((byte)targetPlr.whoAmI);
                    packet.Write(usedFood.type);
                    packet.Write(usedFood.buffType);
                    packet.Write(usedFood.buffTime);
                    packet.Send();
                }
                usedFood.stack--;
                if (usedFood.stack <= 0)
                    usedFood.TurnToAir();
                return true;
            }
        }

        return null;
    }

    public override void HoldItem(Player player)
    {
        if (Main.LocalPlayer == player)
        {
            Item food = GetFood(player);
            if (food != null)
            {
                player.cursorItemIconEnabled = true;
                player.cursorItemIconPush = 8;
                player.cursorItemIconID = food.type;
            }
        }
    }

    public bool WhatKindOfFruitIsThis(Item selectFood)
    {
        if (selectFood.buffType > 0)
            return true;
        if (ItemCategories._weightGainPotions.Contains(selectFood.type) || ItemCategories._weightLossPotions.Contains(selectFood.type))
            return true;
        return false;
    }

    public Item GetFood(Player player)
    {
        Item item = null;
        for (int i = 0; i < 54; i++)
        {
            if (player.inventory[i].stack > 0 && WhatKindOfFruitIsThis(player.inventory[i]))
            {
                item = player.inventory[i];
                break;
            }
        }
        return item;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CopperBar, 6)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public static void HandleWGPotion(Player player, int itemType)
    {
        WeightPotion usedPotion = (WeightPotion)ModContent.GetModItem(itemType);

        float weightChange = usedPotion.WeightEffect;

        if (player.HasBuff<MilkshakeSickness>())
            weightChange *= 0.1f;
        else
            player.AddBuff(ModContent.BuffType<MilkshakeSickness>(), 1 * 60 * 30);
        
        player.Wg().CombatWeightText(player.Wg().AddWeight(weightChange), false);
    }

    public static void HandleSpecialCases(Player player, int itemType)
    {
        if (itemType == ModContent.ItemType<GlowingSoup>())
            player.AddBuff(ModContent.BuffType<Spored>(), Utility.TimeToTicks(minutes: 2));
    }
}

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class TinFunnel : CopperFunnel
{
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TinBar, 6)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
