using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Enemy.Gorgeist;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class TossedPlate : ModProjectile
{
    public static float MaxSpeed = 15;

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;

        Projectile.ai[0] = 1;

        Projectile.height = 24;
        Projectile.width = 24;
    }

    public override void AI()
    {
        Projectile.velocity.X -= 0.3f * Projectile.ai[1];

        Projectile.velocity.Y += (Projectile.ai[0] - Projectile.Center.Y) * 0.01f;
        Projectile.velocity.Y *= 0.7f;
    }
}
