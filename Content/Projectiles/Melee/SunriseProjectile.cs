using WgMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles.Melee;

public class SunriseProjectile : ModProjectile
{
	public bool IsStickingToTarget
	{
		get => Projectile.ai[0] == 1f;
		set => Projectile.ai[0] = value ? 1f : 0f;
	}

	public int TargetWhoAmI
	{
		get => (int)Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

	public int GravityDelayTimer
	{
		get => (int)Projectile.ai[2];
		set => Projectile.ai[2] = value;
	}

	public float StickTimer
	{
		get => Projectile.localAI[0];
		set => Projectile.localAI[0] = value;
	}

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 18;
		Projectile.height = 18;
		Projectile.aiStyle = 0;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 2;
		Projectile.timeLeft = 600;
		Projectile.alpha = 255;
		Projectile.light = 0.5f;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.hide = true;

		DrawOffsetX = -4;
	}

	public const int GravityDelay = 45;

	public override void AI()
	{
		UpdateAlpha();
		if (IsStickingToTarget)
		{
			StickyAI();
		}
		else
		{
			NormalAI();
		}
	}

	public void NormalAI()
	{
		GravityDelayTimer++;

		if (GravityDelayTimer >= GravityDelay)
		{
			GravityDelayTimer = GravityDelay;

			Projectile.velocity.X *= 0.98f;
			Projectile.velocity.Y += 0.35f;
		}

		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

		if (Main.rand.NextBool(3))
		{
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<CutieHeart>(), Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
			dust.velocity += Projectile.velocity * 0.3f;
			dust.velocity *= 0.2f;
		}
		if (Main.rand.NextBool(4))
		{
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<CutieHeart>(),
				0, 0, 254, Scale: 0.3f);
			dust.velocity += Projectile.velocity * 0.5f;
			dust.velocity *= 0.5f;
		}
	}

	public const int StickTime = 60 * 15;
	public void StickyAI()
	{
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		StickTimer += 1f;

		bool hitEffect = StickTimer % 30f == 0f;
		int npcTarget = TargetWhoAmI;
		if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200)
		{
			Projectile.Kill();
		}
		else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
		{
			Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
			Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
			if (hitEffect)
			{
				Main.npc[npcTarget].HitEffect(0, 1.0);
			}
		}
		else
		{
			Projectile.Kill();
		}
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		Vector2 usePos = Projectile.position;

		Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2();
		usePos += rotationVector * 16f;

		for (int i = 0; i < 20; i++)
		{
			Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, ModContent.DustType<CutieHeart>());
			dust.position = (dust.position + Projectile.Center) / 2f;
			dust.velocity += rotationVector * 2f;
			dust.velocity *= 0.5f;
			dust.noGravity = true;
			usePos -= rotationVector * 8f;
		}
	}

	public const int MaxStickingJavelin = 6;
	public readonly Point[] _stickingJavelins = new Point[MaxStickingJavelin];

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		IsStickingToTarget = true;
		TargetWhoAmI = target.whoAmI;
		Projectile.velocity = (target.Center - Projectile.Center) *
			0.75f;
		Projectile.netUpdate = true;
		Projectile.damage = 0;

		target.AddBuff(ModContent.BuffType<Sunrising>(), 900);

		Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, _stickingJavelins);
	}

	public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
	{
		return true;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
		{
			targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
		}
		return projHitbox.Intersects(targetHitbox);
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		if (IsStickingToTarget)
		{
			int npcIndex = TargetWhoAmI;
			if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
			{
				if (Main.npc[npcIndex].behindTiles)
				{
					behindNPCsAndTiles.Add(index);
				}
				else
				{
					behindNPCsAndTiles.Add(index);
				}

				return;
			}
		}
		behindNPCsAndTiles.Add(index);
	}

	public const int AlphaFadeInSpeed = 25;

	public void UpdateAlpha()
	{
		if (Projectile.alpha > 0)
		{
			Projectile.alpha -= AlphaFadeInSpeed;
		}

		if (Projectile.alpha < 0)
		{
			Projectile.alpha = 0;
		}
	}
}
