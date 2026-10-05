using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
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
public class StrawProjectile : ModProjectile
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.alpha = 255;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 12;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Generic;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 12;
        Projectile.ArmorPenetration = 5;
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 direction = Projectile.velocity;
        direction.Normalize();
        return Collision.CheckAABBvLineCollision(new Vector2(targetHitbox.X, targetHitbox.Y), new Vector2(targetHitbox.Width, targetHitbox.Height),
            projHitbox.Center.ToVector2(), projHitbox.Center.ToVector2() + direction * 75);
    }
    public override bool CanHitPvp(Player target)
    {
        return false;
    }
    public override bool? CanHitNPC(NPC target)
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active)
            return false;

        if (owner.TryGetModPlayer(out BuffHitPlayer plr))
        {
            if (plr._slimes.Contains(target.type))
            {
                return null;
            }
        }
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active)
            return;

        if (owner.TryGetModPlayer(out BuffHitPlayer plr))
        {
            if (plr._slimes.Contains(target.type))
            {
                owner.Wg().AddStomach(damageDone / 5f);
                SoundEngine.PlaySound(WgSounds.Gulp, owner.Center);
            }
        }
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead)
            Projectile.Kill();
        else
        {
            Projectile.Center = owner.Center;
        }
    }
}
public class SellStraw : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.PartyGirl)
            shop.Add<DrinkingStraw>();
    }
}
