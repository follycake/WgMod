using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Dusts;

namespace WgMod.Content.Projectiles.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BouncyBall : ModProjectile
{
    NPC HomingTarget
    {
        get => Projectile.ai[1] == 0 ? null : Main.npc[(int)Projectile.ai[1] - 1];
        set { Projectile.ai[1] = value == null ? 0 : value.whoAmI + 1; }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 10;
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] >= 15f)
        {
            Projectile.ai[0] = 15f;
            Projectile.velocity.Y += 0.1f;
        }

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }

        float maxDetectRadius = 400f;

        if (HomingTarget == null)
        {
            HomingTarget = FindClosestNPC(maxDetectRadius);
        }

        if (HomingTarget != null && !IsValidTarget(HomingTarget))
        {
            HomingTarget = null;
        }
    }

    public NPC FindClosestNPC(float maxDetectDistance)
    {
        NPC closestNPC = null;

        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        foreach (var target in Main.ActiveNPCs)
        {
            if (IsValidTarget(target))
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(
                    target.Center,
                    Projectile.Center
                );

                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestNPC = target;
                }
            }
        }

        return closestNPC;
    }

    public bool IsValidTarget(NPC target)
    {
        return target.CanBeChasedBy()
            && Collision.CanHit(
                Projectile.Center,
                1,
                1,
                target.position,
                target.width,
                target.height
            );
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.penetrate--;
        if (Projectile.penetrate <= 0)
        {
            Projectile.Kill();
        }
        else if (HomingTarget != null)
        {
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);

            Projectile.velocity =
                Projectile
                    .velocity.ToRotation()
                    .AngleTowards(targetAngle, MathHelper.ToRadians(360))
                    .ToRotationVector2() * length;
            Projectile.velocity.Y += -oldVelocity.Y * 0.5f;
        }
        else
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.9f;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust cute = Dust.NewDustDirect(
                    Projectile.position,
                    12,
                    12,
                    ModContent.DustType<CutieHeart>(),
                    0,
                    0,
                    100,
                    default,
                    1
                );
                cute.noGravity = true;
            }
        }

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(WgSounds.Squeaky, Projectile.Center);

        Vector2 direction = target.Center.DirectionTo(Projectile.Center);
        float velocity = Projectile.velocity.Length();

        Projectile.velocity = direction * velocity;

        for (int i = 0; i < 5; i++)
        {
            Dust cute = Dust.NewDustDirect(
                Projectile.position,
                12,
                12,
                ModContent.DustType<CutieHeart>(),
                0,
                0,
                100,
                default,
                1
            );
            cute.noGravity = true;
        }

        Projectile.netUpdate = true;
    }

    public override void PrepareBombToBlow()
    {
        Projectile.tileCollide = false;
        Projectile.alpha = 255;

        Projectile.Resize(77, 77);

        if (Main.hardMode)
            Projectile.damage = 777;
        else
            Projectile.damage = 77;

        Projectile.knockBack = 7f;
    }

    public override void OnKill(int timeLeft)
    {
        Projectile.PrepareBombToBlow();
        if (Main.rand.NextBool(3))
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        for (int i = 0; i < 50; i++)
        {
            Dust cute = Dust.NewDustDirect(
                Projectile.position,
                77,
                77,
                DustID.GemRuby,
                0,
                0,
                100,
                default,
                1
            );
            cute.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos =
                Projectile.oldPos[k]
                - Main.screenPosition
                + drawOrigin
                + new Vector2(0f, Projectile.gfxOffY);
            Color color =
                Projectile.GetAlpha(lightColor)
                * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(
                texture,
                drawPos,
                null,
                color,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }

        return true;
    }
}
