using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Pets;

namespace WgMod.Content.Projectiles.Pets;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.drarky)]
public class LilGorgie : ModProjectile
{
	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 2;
		Main.projPet[Type] = true;

		ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 5)
			.WithOffset(-2, -22f)
			.WithCode(CharacterPreviewCustomization);
	}

	public static void CharacterPreviewCustomization(Projectile proj, bool walking)
	{
		float half = 0.5f;
		float timer = (float)Main.timeForVisualEffects % 60f / 60f;
		float speed = 1f;
		proj.position.Y += 0f - half + (float)(Math.Cos(timer * MathHelper.TwoPi * speed) * half * 2f);
	}

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet);

		Projectile.aiStyle = -1;
		Projectile.width = 38;
		Projectile.height = 54;
	}

	public override void AI()
	{
		// Todo: Make LilGorgie move the same way GorgeistBoss does

		Player player = Main.player[Projectile.owner];

		CheckActive(player);

		bool movesFast = Movement(player);

		Animate(movesFast);
	}

	public void CheckActive(Player player)
	{
		if (!player.dead && player.HasBuff(ModContent.BuffType<GorgeistPet>()))
		{
			Projectile.timeLeft = 2;
		}
	}

	public bool Movement(Player player)
	{
		float velDistanceChange = 2f;

		Vector2 desiredCenterRelative = new(20f + player.width / 2, -player.height);

		desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 120f * MathHelper.TwoPi) * 5;

		Vector2 desiredCenter = player.MountedCenter + desiredCenterRelative;
		Vector2 betweenDirection = desiredCenter - Projectile.Center;
		float betweenSQ = betweenDirection.LengthSquared();

		if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange)
		{
			Projectile.Center = desiredCenter;
			Projectile.velocity = Vector2.Zero;
		}

		if (betweenDirection != Vector2.Zero)
		{
			Projectile.velocity = betweenDirection * 0.1f * 2;
		}

		bool movesFast = Projectile.velocity.LengthSquared() > 6f * 6f;

		if (movesFast)
		{
			float rotationVel = Projectile.velocity.X * 0.08f + Projectile.velocity.Y * Projectile.spriteDirection * 0.02f;
			if (Math.Abs(Projectile.rotation - rotationVel) >= MathHelper.Pi)
			{
				if (rotationVel < Projectile.rotation)
				{
					Projectile.rotation -= MathHelper.TwoPi;
				}
				else
				{
					Projectile.rotation += MathHelper.TwoPi;
				}
			}

			float rotationInertia = 12f;
			Projectile.rotation = (Projectile.rotation * (rotationInertia - 1f) + rotationVel) / rotationInertia;
		}
		else
		{
			if (Projectile.rotation > MathHelper.Pi)
			{
				Projectile.rotation -= MathHelper.TwoPi;
			}

			if (Projectile.rotation > -0.005f && Projectile.rotation < 0.005f)
			{
				Projectile.rotation = 0f;
			}
			else
			{
				Projectile.rotation *= 0.96f;
			}
		}

		return movesFast;
	}

	public void Animate(bool movesFast)
	{
		int animationSpeed = 7;

		if (movesFast)
		{
			animationSpeed = 4;
		}

		Projectile.frameCounter++;
		if (Projectile.frameCounter > animationSpeed)
		{
			Projectile.frameCounter = 0;
			Projectile.frame++;

			if (Projectile.frame >= Main.projFrames[Type])
			{
				Projectile.frame = 0;
			}
		}
	}
}
