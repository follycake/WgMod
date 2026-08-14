using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class SterlingPlatterProjectile : ModProjectile
{
	public static int CooldownMax = 36;

	public int _cooldown;

	public bool _exhausted = false;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.LightDisc);
		AIType = ProjectileID.LightDisc;
	}

	public void SpawnSterlingExplosion()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient || _exhausted)
			return;

		Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new(0f, 0f), ModContent.ProjectileType<SterlingExplosion>(), Projectile.damage * 2, Projectile.knockBack * 2);

		_exhausted = true;
	}

	public override void AI()
	{
		_cooldown++;

		if (_cooldown == CooldownMax)
			SpawnSterlingExplosion();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SpawnSterlingExplosion();
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		SpawnSterlingExplosion();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		SpawnSterlingExplosion();

		return true;
	}

	public override void OnKill(int timeLeft)
	{
		SpawnSterlingExplosion();
	}
}
