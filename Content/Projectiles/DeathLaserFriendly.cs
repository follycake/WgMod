using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class DeathLaserFriendly : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.DeathLaser);
        AIType = ProjectileID.DeathLaser;

        Projectile.DamageType = DamageClass.Melee;
        Projectile.friendly = true;
        Projectile.hostile = false;
    }
}
