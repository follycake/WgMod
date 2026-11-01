using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles.Minions;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class IndulgentImpBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = true;
		Main.buffNoTimeDisplay[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<IndulgentImp>()] > 0)
			player.buffTime[buffIndex] = 18000;
		else
		{
			player.DelBuff(buffIndex);
			buffIndex--;
		}
	}
}

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class IndulgentImp : ModProjectile
{
	static readonly int _projectileCooldown = 120;

	static readonly int _projectileRandom = 1 + _projectileCooldown / 10;

	int _shootingAndKillingPeopleForFun = Main.rand.Next(0, _projectileRandom);
	int _damage;

	bool _animate = true;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 11;
		ProjectileID.Sets.MinionTargettingFeature[Type] = true;

		Main.projPet[Type] = true;

		ProjectileID.Sets.MinionSacrificable[Type] = true;
		ProjectileID.Sets.CultistIsResistantTo[Type] = true;
	}

	public sealed override void SetDefaults()
	{
		Projectile.aiStyle = ProjAIStyleID.Hornet;

		DrawOffsetX = -20;
		DrawOriginOffsetY = -6;

		Projectile.width = 48;
		Projectile.height = 52;
		Projectile.tileCollide = false;

		Projectile.friendly = true;
		Projectile.minion = true;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.minionSlots = 1f;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Player owner = Main.player[Projectile.owner];

		if (!owner.TryGetModPlayer(out IndulgentImpController ii))
			return;

		if (ii.Stage > 1)
			Projectile.Kill();
		else
			_damage = Projectile.damage;
	}

	public override bool? CanCutTiles()
	{
		return false;
	}

	public override bool MinionContactDamage()
	{
		return true;
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];

		if (!owner.TryGetModPlayer(out IndulgentImpController ii) || !CheckActive(owner))
			return;

		int frameSpeed = 15;
		int startFrame = 0;
		int endFrame = 3;

		NPC target = Projectile.FindTargetWithinRange(800);

		switch (ii.Stage)
		{
			case 1:
				Projectile.aiStyle = ProjAIStyleID.Hornet;
				Projectile.damage = _damage;
				break;
			case 2:
				Projectile.aiStyle = ProjAIStyleID.Hornet;
				Projectile.damage = _damage * 2;
				Projectile.minionSlots = 2;

				startFrame = 3;
				endFrame = 6;
				break;
			case 3:
				Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
				Projectile.damage = _damage * 4;

				Projectile.minion = false;
				Projectile.minionSlots = 0;
				Projectile.sentry = true;
				Projectile.netImportant = true;
				owner.UpdateMaxTurrets();

				DrawOffsetX = -6;

				Projectile.width = 78;
				Projectile.height = 54;

				startFrame = 6;
				endFrame = 11;

				if (Projectile.Distance(owner.Center) > 800 || ii.QuickTeleport)
				{
					Vector2 offset = new(0, -20);
					Vector2 tpSpot = owner.Center + offset;

					if (ii.QuickTeleport)
						tpSpot = Main.MouseWorld;

					TeleportEffects(Projectile.position);

					Projectile.Center = tpSpot;
					Projectile.direction = owner.direction;

					TeleportEffects(Projectile.position);

					ii.QuickTeleport = false;
				}
				break;
		}

		if (target != null)
		{
			if (_shootingAndKillingPeopleForFun <= _projectileCooldown)
				_shootingAndKillingPeopleForFun++;
			else
			{
				_shootingAndKillingPeopleForFun = Main.rand.Next(0, _projectileRandom);

				if (Main.myPlayer == Projectile.owner)
				{
					Vector2 direction = Projectile.Center.DirectionTo(target.Center);

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 12f, ProjectileID.ImpFireball, Projectile.damage, Projectile.knockBack);
				}
			}

			if (ii.Stage == 3)
				_animate = true;
		}
		else if (ii.Stage == 3)
			_animate = false;

		Projectile.rotation = Projectile.velocity.X * 0.05f;

		if (ii.Stage != ii.PrevStage)
			Projectile.frame = startFrame;

		ii.PrevStage = ii.Stage;

		if (_animate)
			Projectile.frameCounter++;
		else
		{
			Projectile.frameCounter = 0;
			Projectile.frame = startFrame;
		}

		if (Projectile.frameCounter >= frameSpeed)
		{
			Projectile.frameCounter = startFrame;
			Projectile.frame++;

			if (Projectile.frame >= endFrame)
				Projectile.frame = startFrame;
		}

		if (Main.rand.NextBool(6))
		{
			Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, Alpha: 100, newColor: new Color(), Scale: 2f);

			dust.velocity *= 0.3f;
			dust.noGravity = true;
			dust.noLight = true;
		}
	}

	void TeleportEffects(Vector2 position)
	{
		SoundEngine.PlaySound(SoundID.Item8, position);

		for (int i = 0; i < 50; i++)
		{
			Dust fire = Dust.NewDustDirect(position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, default, default, 2f);

			fire.noGravity = true;
		}
	}

	bool CheckActive(Player owner)
	{
		if (owner.dead || !owner.active)
		{
			owner.ClearBuff(ModContent.BuffType<IndulgentImpBuff>());

			return false;
		}

		if (owner.HasBuff(ModContent.BuffType<IndulgentImpBuff>()))
			Projectile.timeLeft = 2;

		return true;
	}
}

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class IndulgentImpController : ModPlayer
{
	public int Stage;
	public int PrevStage;
	public bool QuickTeleport;

	public override void PreUpdateBuffs()
	{
		Player player = Main.LocalPlayer;

		if (Player != player)
			return;

		if (!player.HasBuff(ModContent.BuffType<IndulgentImpBuff>()))
			Stage = 0;
	}
}
