using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles;

public class HoneySprayProjectile : ModProjectile
{

    // Use the vanilla Slime Gun projectile texture.
    public override string Texture =>
        $"Terraria/Images/Projectile_{ProjectileID.SlimeGun}";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;

        Projectile.aiStyle = 0;

        /*
         * This projectile never enters Terraria's damaging
         * NPC/player collision system.
         */
        Projectile.friendly = false;
        Projectile.hostile = false;

        Projectile.damage = 0;
        Projectile.knockBack = 0f;

        Projectile.timeLeft = 120;

        /*
         * Disable tile collision while diagnosing the spawn.
         * This guarantees that it cannot immediately die from
         * appearing next to a wall or inside a tile.
         */
        Projectile.tileCollide = true;

        Projectile.ignoreWater = false;

        Projectile.alpha = 0;
        Projectile.scale = 1f;
        Projectile.hide = false;

        Projectile.penetrate = -1;
    }

    //public override bool? CanDamage()
    //{
    //return false;
    //}

    public override void OnSpawn(IEntitySource source)
    {
        /*
         * This proves the projectile object was created.
         * Remove after testing.
         */
        if (Projectile.owner == Main.myPlayer)
        {

        }
    }

    public override void AI()
    {
        /*
         * Simple droplet physics.
         */
        Projectile.velocity.X *= 0.985f;
        Projectile.velocity.Y += 0.08f;

        if (Projectile.velocity.Y > 20f)
        {
            Projectile.velocity.Y = 20f;
        }

        Projectile.rotation =
            Projectile.velocity.ToRotation() +
            MathHelper.PiOver2;

        /*
         * Bright visual debugging.
         *
         * Even if the projectile texture has a drawing issue,
         * this dust will reveal where the projectile is.
         */
        Dust dust = Dust.NewDustPerfect(
            Projectile.Center,
            DustID.Poisoned,
            -Projectile.velocity * 0.1f,
            Scale: 1.25f
        );

        dust.noGravity = true;

        Lighting.AddLight(
            Projectile.Center,
            0.1f,
            0.5f,
            0.1f
        );

        /*
         * Only the owning client handles manual contact.
         * Projectile AI itself runs in several multiplayer contexts,
         * so this prevents duplicate applications.
         */
        if (Projectile.owner == Main.myPlayer)
        {
            CheckEntityCollisions();
        }
    }

    void CheckEntityCollisions()
    {
        Rectangle effectHitbox = Projectile.Hitbox;

        /*
         * Make the status-effect contact area slightly larger
         * than the visible droplet.
         */
        effectHitbox.Inflate(6, 6);

        if (TryAffectNPC(effectHitbox))
        {
            Projectile.Kill();
            return;
        }

        if (TryAffectPlayer(effectHitbox))
        {
            Projectile.Kill();
        }
    }

    bool TryAffectNPC(Rectangle effectHitbox)
    {
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC target = Main.npc[i];

            if (!target.active)
                continue;

            if (target.friendly)
                continue;

            if (target.dontTakeDamage)
                continue;

            if (target.lifeMax <= 5)
                continue;

            if (!effectHitbox.Intersects(target.Hitbox))
                continue;



            return true;
        }

        return false;
    }

    bool TryAffectPlayer(Rectangle effectHitbox)
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player target = Main.player[i];

            if (!target.active || target.dead)
                continue;

            // Prevent immediate self-poisoning.
            if (target.whoAmI == Projectile.owner)
                continue;

            if (!effectHitbox.Intersects(target.Hitbox))
                continue;

            target.AddBuff(ModContent.BuffType<ForceFed>(), (int)(ForceFed.TicksPerCycle * 1.5f), quiet: false);
            target.AddBuff(BuffID.WellFed, 60 * 4, quiet: false);

            return true;
        }

        return false;
    }
}
