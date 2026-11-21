using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles.Magic;

namespace WgMod.Content.Items.Weapons.Magic;

public class CoolFuckingOrb : ModItem
{
    const int CooldownMax = 2 * 60;

    int _cooldown = CooldownMax;

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.LightRed;

        Item.useTime = 12;
        Item.useAnimation = 12;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.autoReuse = true;

        Item.DamageType = DamageClass.Ranged;
        Item.damage = 35;
        Item.knockBack = 6f;
        Item.noMelee = true;
        Item.mana = 50;

        Item.shoot = ModContent.ProjectileType<SmartShot>();
        Item.shootSpeed = 12f;
    }

    public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
    {
        FindClosestNPC(player, 2000f, out NPC closestNPC);

        if (closestNPC == null || _cooldown != CooldownMax)
            mult = 0;
    }

    public override void UpdateInventory(Player player)
    {
        if (_cooldown < CooldownMax)
            _cooldown++;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        FindClosestNPC(player, 2000f, out NPC closestNPC);

        if (closestNPC == null || _cooldown != CooldownMax)
        {
            type = 0;
            return;
        }

        _cooldown = 0;

        float length = velocity.Length();
        float targetAngle = position.AngleTo(closestNPC.Center);

        velocity = velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(360)).ToRotationVector2() * length;
    }

    public static NPC FindClosestNPC(Player owner, float maxDetectDistance, out NPC closestNPC)
    {
        closestNPC = null;

        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        foreach (NPC target in Main.ActiveNPCs)
        {
            if (IsValidTarget(owner, target))
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, owner.Center);

                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }
        }

        return closestNPC;
    }

    public static bool IsValidTarget(Player owner, NPC target)
    {
        return target.CanBeChasedBy() && Collision.CanHit(owner.Center, 1, 1, target.position, target.width, target.height);
    }
}