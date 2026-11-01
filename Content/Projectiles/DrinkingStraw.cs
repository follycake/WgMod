using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using WgMod;
using WgMod.Common.Players;

namespace WgMod.Content.Projectiles;

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
