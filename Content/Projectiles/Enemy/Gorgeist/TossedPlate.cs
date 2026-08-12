using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Enemy.Gorgeist;

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
        Projectile.width = 74;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.velocity.X = 15f;
    }

    public override void AI()
    {
        Projectile.velocity.X -= 0.2f * Projectile.ai[1];

        Projectile.velocity.Y += (Projectile.ai[0] - Projectile.Center.Y) * 0.01f;
        Projectile.velocity.Y *= 0.7f;
    }
}
