using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class FatteningDart : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.PoisonDartTrap);
        Projectile.aiStyle = 0;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 0)
        {
            SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            Projectile.ai[0] = 1;
        }
    }

    public override void PostAI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.Wg().AddWeight(Math.Clamp(info.Damage / 4f, 1, 10));
        target.AddBuff(ModContent.BuffType<ForceFed>(), 120, false);
    }
}
