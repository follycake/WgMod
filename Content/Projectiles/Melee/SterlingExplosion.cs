using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class SterlingExplosion : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.height = 96;
        Projectile.width = 96;

        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.DamageType = DamageClass.Melee;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;

        Projectile.timeLeft = 15;
    }

    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
    }

    public override void AI()
    {
        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(3))
            target.AddBuff(ModContent.BuffType<SterlingSplinters>(), 4 * 60);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        if (Main.rand.NextBool(3))
            target.AddBuff(ModContent.BuffType<SterlingSplinters>(), 2 * 60);
    }
}
