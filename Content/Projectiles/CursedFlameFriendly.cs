using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class CursedFlameFriendly : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.CursedFlameHostile);
        AIType = ProjectileID.CursedFlameHostile;

        Projectile.DamageType = DamageClass.Melee;
        Projectile.friendly = true;
        Projectile.hostile = false;
    }
}
