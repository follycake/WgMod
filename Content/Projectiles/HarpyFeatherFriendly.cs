using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public class HarpyFeatherFriendly : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.HarpyFeather);
        AIType = ProjectileID.HarpyFeather;

        Projectile.friendly = true;
        Projectile.hostile = false;
    }
}
