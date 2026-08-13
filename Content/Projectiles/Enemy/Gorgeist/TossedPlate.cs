using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Enemy.Gorgeist;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class TossedPlate : ModProjectile
{
    public static int Death = 3 * 60;

    public int _deathTimer;
    public int _soundCooldown;

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

        if (_deathTimer < Death)
            _deathTimer++;
        else
            Projectile.Kill();

        if (_soundCooldown < 10)
            _soundCooldown++;
        else
        {
            SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
            _soundCooldown = 0;
        }
    }


    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
        for (int i = 0; i < 15; i++)
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Glass);
        }
    }
}
