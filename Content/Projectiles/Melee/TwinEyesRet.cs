using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace WgMod.Content.Projectiles.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class TwinEyesRet : ModProjectile
{
	const string ChainTexturePath = "WgMod/Content/Projectiles/Melee/TwinEyesChain";

	static Asset<Texture2D> _chainTexture;

	public int _fireCooldown;
	public int projectile;
	public int dust;

	public bool _missFire = false;

	public const int FireCooldownMax = 18;

	enum AIState
	{
		Spinning,
		LaunchingForward,
		Retracting,
		UnusedState,
		ForcedRetracting,
		Ricochet,
		Dropping
	}

	AIState CurrentAIState
	{
		get => (AIState)Projectile.ai[0];
		set => Projectile.ai[0] = (float)value;
	}

	public ref float StateTimer => ref Projectile.ai[1];
	public ref float CollisionCounter => ref Projectile.localAI[0];
	public ref float SpinningStateTimer => ref Projectile.localAI[1];

	public override void Load()
	{
		_chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
	}

	public override void SetStaticDefaults()
	{
		// These lines facilitate the trail drawing
		ProjectileID.Sets.TrailCacheLength[Type] = 6;
		ProjectileID.Sets.TrailingMode[Type] = 2;
	}

	public override void SetDefaults()
	{
		Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
		Projectile.width = 22; // The width of your projectile
		Projectile.height = 22; // The height of your projectile
		Projectile.friendly = true; // Deals damage to enemies
		Projectile.penetrate = -1; // Infinite pierce
		Projectile.DamageType = DamageClass.Melee; // Deals melee damage
		Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
		Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic

		DrawOffsetX = -8;
		DrawOriginOffsetY = -8;

		Projectile.ai[2] = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		if (Projectile.ai[2] == 0)
			Projectile.NewProjectile(source, Projectile.position, Projectile.velocity, ModContent.ProjectileType<TwinEyesSpazm>(), Projectile.damage, Projectile.knockBack, default, default, default, 1);

		if (Projectile.ai[2] == 1)
		{
			projectile = ModContent.ProjectileType<CursedFlameFriendly>();
			dust = DustID.CursedTorch;
		}
		else
		{
			projectile = ModContent.ProjectileType<DeathLaserFriendly>();
			dust = DustID.RedTorch;
		}
	}

	public void FireProjectile(int type)
	{
		if (Main.myPlayer == Projectile.owner && _fireCooldown == FireCooldownMax && !_missFire)
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 2, type, Projectile.damage, Projectile.knockBack, Main.myPlayer);
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];

		if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
		{
			Projectile.Kill();
			return;
		}

		if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
		{
			Projectile.Kill();
			return;
		}

		Vector2 mountedCenter = player.MountedCenter;
		bool doFastThrowDust = false;
		bool shouldOwnerHitCheck = false;

		int launchTimeLimit = 15;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)

		float launchSpeed = 24f; // How fast the projectile can move
		float maxLaunchLength = 1400f; // How far the projectile's chain can stretch before being forced to retract when in launched state
		float retractAcceleration = 8f; // How quickly the projectile will accelerate back towards the player while retracting
		float maxRetractSpeed = 24f; // The max speed the projectile will have while retracting
		float forcedRetractAcceleration = 12f; // How quickly the projectile will accelerate back towards the player while being forced to retract
		float maxForcedRetractSpeed = 32f; // The max speed the projectile will have while being forced to retract
		float unusedRetractAcceleration = 1f;
		float unusedMaxRetractSpeed = 14f;

		int unusedChainLength = 80;
		int defaultHitCooldown = 5; // How often your flail hits when resting on the ground, or retracting
		int spinHitCooldown = 15; // How often your flail hits when spinning
		int movingHitCooldown = 5; // How often your flail hits when moving
		int ricochetTimeLimit = launchTimeLimit + 5;

		// Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
		float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);

		launchSpeed *= meleeSpeedMultiplier;
		unusedRetractAcceleration *= meleeSpeedMultiplier;
		unusedMaxRetractSpeed *= meleeSpeedMultiplier;
		retractAcceleration *= meleeSpeedMultiplier;
		maxRetractSpeed *= meleeSpeedMultiplier;
		forcedRetractAcceleration *= meleeSpeedMultiplier;
		maxForcedRetractSpeed *= meleeSpeedMultiplier;

		float launchRange = launchSpeed * launchTimeLimit;
		float maxDroppedRange = launchRange + 160f;

		Projectile.localNPCHitCooldown = defaultHitCooldown;

		if (_fireCooldown < FireCooldownMax && !_missFire)
			_fireCooldown++;

		if (_fireCooldown == FireCooldownMax - 1 && !_missFire)
		{
			SoundEngine.PlaySound(SoundID.MaxMana, Projectile.position);

			for (int i = 0; i < 5; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust, 0f, 0f, 150, default, 2.3f);
			}
		}

		switch (CurrentAIState)
		{
			case AIState.Spinning:
				{
					shouldOwnerHitCheck = true;
					if (Projectile.owner == Main.myPlayer)
					{
						Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
						player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
						if (!player.channel) // If the player releases then change to moving forward mode
						{
							CurrentAIState = AIState.LaunchingForward;
							StateTimer = 0f;
							Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
							Projectile.Center = mountedCenter;
							Projectile.netUpdate = true;
							Projectile.ResetLocalNPCHitImmunity();
							Projectile.localNPCHitCooldown = movingHitCooldown;

							if (Projectile.ai[2] == 0)
								Projectile.velocity = Projectile.velocity.RotatedBy(0.1);

							if (Projectile.ai[2] == 1)
								Projectile.velocity = Projectile.velocity.RotatedBy(-0.1);
							break;
						}
					}
					SpinningStateTimer += 1f;
					// This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
					Vector2 offsetFromPlayer;
					if (Projectile.ai[2] == 1)
						offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * -player.direction);
					else
						offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 8f * (SpinningStateTimer / 60f) * player.direction);

					offsetFromPlayer.Y *= 0.8f;
					if (offsetFromPlayer.Y * player.gravDir > 0f)
					{
						offsetFromPlayer.Y *= 0.5f;
					}
					Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
					Projectile.velocity = Vector2.Zero;
					Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
					break;
				}
			case AIState.LaunchingForward:
				{
					doFastThrowDust = true;
					bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
					shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
					if (player.controlUseItem) // If the player clicks, transition to the Dropping state
					{
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
						Projectile.velocity *= 0.2f;
						FireProjectile(projectile);
						break;
					}
					if (shouldSwitchToRetracting)
					{
						CurrentAIState = AIState.Retracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
						Projectile.velocity *= 0.3f;
						FireProjectile(projectile);
					}
					player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					Projectile.localNPCHitCooldown = movingHitCooldown;
					break;
				}
			case AIState.Retracting:
				{
					Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
					if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
					{
						Projectile.Kill(); // Kill the projectile once it is close enough to the player
						return;
					}
					if (player.controlUseItem) // If the player clicks, transition to the Dropping state
					{
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
						Projectile.velocity *= 0.2f;
					}
					else
					{
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}

					_missFire = true;
					break;
				}
			// Projectile.ai[0] == 3; This case is actually unused, but maybe a Terraria update will add it back in, or maybe it is useless, so I left it here.
			case AIState.UnusedState:
				{
					if (!player.controlUseItem)
					{
						CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
						StateTimer = 0f;
						Projectile.netUpdate = true;
						break;
					}
					float currentChainLength = Projectile.Distance(mountedCenter);
					Projectile.tileCollide = StateTimer == 1f;
					bool flag3 = currentChainLength <= launchRange;
					if (flag3 != Projectile.tileCollide)
					{
						Projectile.tileCollide = flag3;
						StateTimer = Projectile.tileCollide ? 1 : 0;
						Projectile.netUpdate = true;
					}
					if (currentChainLength > unusedChainLength)
					{

						if (currentChainLength >= launchRange)
						{
							Projectile.velocity *= 0.5f;
							Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
					}
					else
					{
						if (Projectile.velocity.Length() < 6f)
						{
							Projectile.velocity.X *= 0.96f;
							Projectile.velocity.Y += 0.2f;
						}
						if (player.velocity.X == 0f)
						{
							Projectile.velocity.X *= 0.96f;
						}
					}
					player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					break;
				}
			case AIState.ForcedRetracting:
				{
					Projectile.tileCollide = false;
					Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
					if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
					{
						Projectile.Kill(); // Kill the projectile once it is close enough to the player
						return;
					}
					Projectile.velocity *= 0.98f;
					Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
					Vector2 target = Projectile.Center + Projectile.velocity;
					Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
					if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
					{
						Projectile.Kill(); // Kill projectile if it will pass the player
						return;
					}
					player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());

					_missFire = true;
					break;
				}
			case AIState.Ricochet:
				if (StateTimer++ >= ricochetTimeLimit)
				{
					CurrentAIState = AIState.Dropping;
					StateTimer = 0f;
					Projectile.netUpdate = true;
				}
				else
				{
					Projectile.localNPCHitCooldown = movingHitCooldown;
					Projectile.velocity.Y += 0.6f;
					Projectile.velocity.X *= 0.95f;
					player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
				}

				_missFire = true;
				break;
			case AIState.Dropping:
				if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange)
				{
					CurrentAIState = AIState.ForcedRetracting;
					StateTimer = 0f;
					Projectile.netUpdate = true;
				}
				else
				{
					Projectile.velocity.Y += 0.8f;
					Projectile.velocity.X *= 0.95f;
					player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
				}

				_missFire = true;
				break;
		}

		// This is where Flower Pow launches projectiles. Decompile Terraria to view that code.

		Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
		Projectile.spriteDirection = Projectile.direction;
		Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.

		// This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.

		/*
		bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
		if (freeRotation)
		{
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
		}
		else
		{
			Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
			Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
		}
        */
		switch (CurrentAIState)
		{
			case AIState.Dropping:
				Projectile.rotation += Projectile.velocity.X * 0.1f;
				break;
			case AIState.Spinning:
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.Pi;
				break;
			case AIState.Retracting or AIState.ForcedRetracting:
				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.spriteDirection = 1;
				break;
			default:
				Projectile.rotation = Projectile.velocity.ToRotation();
				Projectile.spriteDirection = -1;
				break;
		}

		// If you have a ball shaped flail, you can use this simplified rotation code instead
		/*
		if (Projectile.velocity.Length() > 1f)
			Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
		else
			Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
		*/

		Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
		player.heldProj = Projectile.whoAmI;
		player.SetDummyItemTime(2); // Add a delay so the player can't button mash the flail
		player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
		if (Projectile.Center.X < mountedCenter.X)
		{
			player.itemRotation += (float)Math.PI;
		}
		player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

		// Spawning dust. We spawn dust more often when in the LaunchingForward state
		int dustRate = 15;
		if (doFastThrowDust)
			dustRate = 1;

		if (Main.rand.NextBool(dustRate))
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dust, 0f, 0f, 150, default, 1.3f);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		int defaultLocalNPCHitCooldown = 10;
		int impactIntensity = 0;

		Vector2 velocity = Projectile.velocity;

		float bounceFactor = 0.2f;

		if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
			bounceFactor = 0.4f;

		if (CurrentAIState == AIState.Dropping)
			bounceFactor = 0f;

		if (oldVelocity.X != Projectile.velocity.X)
		{
			if (Math.Abs(oldVelocity.X) > 4f)
				impactIntensity = 1;

			Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
			CollisionCounter += 1f;
		}

		if (oldVelocity.Y != Projectile.velocity.Y)
		{
			if (Math.Abs(oldVelocity.Y) > 4f)
				impactIntensity = 1;

			Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
			CollisionCounter += 1f;
		}

		// If in the Launched state, spawn sparks
		if (CurrentAIState == AIState.LaunchingForward)
		{
			CurrentAIState = AIState.Ricochet;

			Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
			Projectile.netUpdate = true;

			Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
			Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();

			impactIntensity = 2;

			Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
			Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);

			Projectile.position -= velocity;
		}

		// Here the tiles spawn dust indicating they've been hit
		if (impactIntensity > 0)
		{
			Projectile.netUpdate = true;
			for (int i = 0; i < impactIntensity; i++)
				Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);

			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		}

		// Force retraction if stuck on tiles while retracting
		if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
		{
			CurrentAIState = AIState.ForcedRetracting;
			Projectile.netUpdate = true;
		}

		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (Projectile.ai[2] == 1 && Main.rand.NextBool(2))
			target.AddBuff(BuffID.CursedInferno, 5 * 60);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (Projectile.ai[2] == 1 && Main.rand.NextBool(4))
			target.AddBuff(BuffID.CursedInferno, 3 * 60);
	}

	public override bool? CanDamage()
	{
		// Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
		if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
			return false;

		return base.CanDamage();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		// Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistent and not reliant of the actual position of the flail projectile.
		if (CurrentAIState == AIState.Spinning)
		{
			Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
			shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
			float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
			return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
		}
		// Regular collision logic happens otherwise.
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.

		// Flails do 20% more damage while spinning
		if (CurrentAIState == AIState.Spinning)
			modifiers.SourceDamage *= 1.2f;
		// Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
			modifiers.SourceDamage *= 2f;

		// The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
		modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

		// Knockback is only 25% as powerful when in spin mode
		if (CurrentAIState == AIState.Spinning)
			modifiers.Knockback *= 0.25f;
		// Knockback is only 50% as powerful when in drop down mode
		else if (CurrentAIState == AIState.Dropping)
			modifiers.Knockback *= 0.5f;
	}

	// PreDraw is used to draw a chain and trail before the projectile is drawn normally.
	public override bool PreDraw(ref Color lightColor)
	{
		Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
		Rectangle? chainSourceRectangle = null;
		// Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
		float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap.

		Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (_chainTexture.Size() / 2f);
		Vector2 chainDrawPosition = Projectile.Center;
		Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
		Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
		float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : _chainTexture.Height()) + chainHeightAdjustment;

		if (chainSegmentLength == 0)
			chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.

		float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
		int chainCount = 0;
		float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

		// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
		while (chainLengthRemainingToDraw > 0f)
		{
			// This code gets the lighting at the current tile coordinates
			Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

			// Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
			// Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
			// This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

			var chainTextureToDraw = _chainTexture;

			if (chainCount >= 4)
			{ }
			else if (chainCount >= 2)
			{
				byte minValue = 140;
				if (chainDrawColor.R < minValue)
					chainDrawColor.R = minValue;

				if (chainDrawColor.G < minValue)
					chainDrawColor.G = minValue;

				if (chainDrawColor.B < minValue)
					chainDrawColor.B = minValue;
			}
			else
				chainDrawColor = Color.White;

			// Here, we draw the chain texture at the coordinates
			Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

			// chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
			chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
			chainCount++;
			chainLengthRemainingToDraw -= chainSegmentLength;
		}

		// Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)

		Texture2D projectileTexture = TextureAssets.Projectile[Type].Value;
		Vector2 drawOrigin = new(projectileTexture.Width * 0.5f, projectileTexture.Height * 0.5f);
		SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		if (CurrentAIState == AIState.LaunchingForward)
		{
			int afterimageCount = Math.Min(Projectile.oldPos.Length - 1, (int)StateTimer);
			for (int k = afterimageCount; k > 0; k--)
			{
				Vector2 trailDrawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
				Main.spriteBatch.Draw(projectileTexture, trailDrawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
			}
		}

		Vector2 drawPos = Projectile.position - Main.screenPosition + new Vector2(Projectile.width / 2, Projectile.height / 2) + new Vector2(0f, Projectile.gfxOffY);

		Main.spriteBatch.Draw(projectileTexture, drawPos, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

		return false;
	}
}
