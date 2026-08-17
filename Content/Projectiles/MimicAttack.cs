using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Achievements;

namespace WgMod.Content.Projectiles;

public class MimicAttack : ModProjectile
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 5;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (target.life - damageDone <= 0 && target.boss)
            Playtime.Condition.Complete();
    }
}
