using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;
using WgMod.Content.TileEntities;

namespace WgMod.Content.Items;

public class DrinkingStraw : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToStaff(ModContent.ProjectileType<StrawProjectile>(), 0.01f, 12, 0);
        Item.DamageType = DamageClass.Generic;
        Item.damage = 12;
        Item.UseSound = null;
        Item.value = Item.buyPrice(silver: 3, copper: 50);
    }

    public override bool? UseItem(Player player)
    {
        if (player != Main.LocalPlayer)
            return null;

        Vector2 selectTile = Main.MouseWorld;

        if (player.IsTargetTileInItemRange(Item))
        {
            Point16 targetPos = Utils.ToTileCoordinates16(selectTile);

            Tile targetTile = Main.tile[targetPos.X, targetPos.Y];

            if (targetTile.LiquidAmount == 0)
                return null;

            int drankLiquid = targetTile.LiquidType;
            byte liquidLeft = (byte)Math.Max(targetTile.LiquidAmount - 200, 0);
            int totalDrank = 200;
            if (liquidLeft == 0)
                totalDrank -= targetTile.LiquidAmount;

            targetTile.LiquidAmount = liquidLeft;

            TEFeedingTube.FluidInfo fluidInfo = TEFeedingTube.FluidTable[drankLiquid];
            player.Wg().AddStomach(fluidInfo.Gain * totalDrank / 255);

            SoundEngine.PlaySound(WgSounds.Gulp, player.Center);

            if (liquidLeft == 0)
                targetTile.LiquidType = LiquidID.Water; //i guess this is the default?

            if (drankLiquid == LiquidID.Lava)
                player.TouchLava();

            WorldGen.SquareTileFrame(targetPos.X, targetPos.Y, false);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.sendWater(targetPos.X, targetPos.Y);
            }
            else
            {
                Liquid.AddWater(targetPos.X, targetPos.Y);
            }
            return true;
        }

        return null;
    }
}
