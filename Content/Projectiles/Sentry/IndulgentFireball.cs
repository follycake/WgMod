using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Sentry;

public class IndulgentFireball : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.SentryShot[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.ImpFireball);
        AIType = ProjectileID.ImpFireball;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 60 * Main.rand.Next(3, 7));
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffID.OnFire3, 60 * Main.rand.Next(1, 4));
    }
}
