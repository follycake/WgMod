using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;

namespace WgMod.Content.Projectiles.Ranged;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class Plate : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = ProjAIStyleID.Arrow;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Projectile.velocity.X > 0)
			Projectile.spriteDirection = -1;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 10; i++)
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Marble, 0f, 0f, 100, default, 1);

		SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
	}
}