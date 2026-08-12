using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Ranged;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class SporeCloud : ModProjectile
{
    public const int _cooldownMax = 30;
    public int _cooldown = _cooldownMax;
    public int _spinnyDirection;
    public float _spinnySpeed;
    public float _time;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 20;
    }

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 160;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;

        Projectile.penetrate = -1;
        Projectile.timeLeft = 8 * 60;
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = float.Lerp(0, MathF.Tau, Main.rand.NextFloat());
        _spinnySpeed = float.Lerp(0.005f, 0.025f, Main.rand.NextFloat());
        _spinnyDirection = Main.rand.NextFromList(-1, 1);
    }

    public override void AI()
    {
        if (_cooldown < _cooldownMax)
            _cooldown++;
        else
            _cooldown = 0;

        Projectile.rotation += _spinnySpeed * _spinnyDirection;

        _time += 0.0075f;

        float t = MathF.Sin(_time);

        Projectile.Opacity = t;
        Projectile.scale = t;

        /*Projectile.position = Projectile.Center; 
        Projectile.Size = new Vector2(160f, 160f) * t; // If someone else wants to try this go ahead, I give up
        Projectile.Center = Projectile.position;*/

        if (Main.rand.NextBool(2))
        {
            float angle = (float)(Main.rand.NextDouble() * Math.Tau);
            Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));

            Dust.NewDustDirect(new Vector2(Projectile.Center.X - 80f * t, Projectile.Center.Y - 80f * t), (int)(160f * t), (int)(160f * t), DustID.GlowingMushroom, dir.X * t, dir.Y * t, (int)(100f * t), default, t);
        }

        if (_time > MathF.PI)
            Projectile.Kill();

        Projectile.frameCounter++;
        if (Projectile.frameCounter > 0)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            Projectile.frame %= 20;
        }
    }

    public override bool? CanDamage()
    {
        return _cooldown == 0;
    }
}
