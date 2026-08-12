using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Content.Projectiles.Melee;

[Credit(ProjectRole.Programmer, Contributor.alphas0)]
public abstract class WeightPotionProjectile : ModProjectile
{
    public abstract int PotionDamageModifier { get; }
    public abstract float PotionWeightModifier { get; }
    public abstract int PotionExplosionRadius { get; }
    public abstract int SpriteWidth { get; }
    public abstract int SpriteHeight { get; }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true; // Damage dealt to players does not scale with difficulty in vanilla.
    }

    public override void SetDefaults()
    {
        // Projectile size is half the sprite's to avoid problems such as too big a sprite touching the ground immediately on launch and breaking
        Projectile.width = SpriteWidth / 5 * 3;
        Projectile.height = SpriteHeight / 5 * 3;
        Projectile.friendly = true;
        Projectile.penetrate = 1;

        // These help the projectile hitbox be centered on the projectile sprite.
        //DrawOffsetX = -SpriteWidth/2;
        DrawOriginOffsetY = -(SpriteHeight / 5) * 2;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // Vanilla explosions do less damage to Eater of Worlds in expert mode, so we will too.
        if (Main.expertMode)
        {
            if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                modifiers.FinalDamage /= 5;
        }
    }

    public override void AI()
    {
        // The projectile is in the midst of exploding during the last 3 updates.
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            Projectile.PrepareBombToBlow();

        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] > 10f)
        {
            Projectile.ai[0] = 10f;
            if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.96f;
                if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                {
                    Projectile.velocity.X = 0f;
                    Projectile.netUpdate = true;
                }
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
        }
        Projectile.rotation += Projectile.velocity.X * 0.05f;
    }

    public override void PrepareBombToBlow()
    {
        Projectile.tileCollide = false;
        Projectile.alpha = 255;

        Projectile.Resize(PotionExplosionRadius * 16, PotionExplosionRadius * 16);

        //Projectile.damage = 100; // Bomb: 100, Dynamite: 250
        //Projectile.knockBack = PotionExplosionRadius * 0.5f; // Bomb: 8f, Dynamite: 10f
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);

        for (int i = 0; i < 50; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
            dust.velocity *= 1.4f;
        }

        float goreMultiplayer = PotionExplosionRadius / 2;
        for (int g = 0; g < 2; g++)
        {
            var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
            gore.scale = goreMultiplayer;
            gore.velocity.X += goreMultiplayer;
            gore.velocity.Y += goreMultiplayer;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
            gore.scale = goreMultiplayer;
            gore.velocity.X -= goreMultiplayer;
            gore.velocity.Y += goreMultiplayer;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
            gore.scale = goreMultiplayer;
            gore.velocity.X += goreMultiplayer;
            gore.velocity.Y -= goreMultiplayer;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
            gore.scale = goreMultiplayer;
            gore.velocity.X -= goreMultiplayer;
            gore.velocity.Y -= goreMultiplayer;
        }

        int explosionRadius = PotionExplosionRadius;

        // Go through active players and see who is in the range of the explosion then make them gain
        foreach (Player activePlayer in Main.ActivePlayers)
        {
            if (activePlayer.Center.Distance(Projectile.Center) < explosionRadius * 16)
            {
                //activePlayer.Heal((int)((float)100 * PotionDamageModifier));
                if (activePlayer.TryGetModPlayer(out WgPlayer wg))
                    wg.CombatWeightText(wg.AddWeight(10 * PotionWeightModifier), false);
            }
        }

        // Actual explosion effect that hurts mobs
        foreach (NPC activeMob in Main.ActiveNPCs)
        {
            if (!activeMob.friendly && activeMob.Center.Distance(Projectile.Center) < explosionRadius * 16)
            {
                int dir = 1;
                if (activeMob.Center.X < Projectile.Center.X)
                    dir = -1;

                bool crit = false;
                if (Main.rand.Next(1, 100) < Projectile.CritChance)
                    crit = true;

                int damageDealt = (int)((float)Projectile.damage * Math.Abs(PotionDamageModifier));
                activeMob.StrikeNPC(activeMob.CalculateHitInfo(damageDealt, dir, crit, Projectile.knockBack, DamageClass.Melee, true, 0));
            }
        }
    }

    public class LesserWeightGainPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<LesserWeightGainPotion>().Texture;
        public override int PotionDamageModifier => 1;
        public override float PotionWeightModifier => 1f;
        public override int PotionExplosionRadius => 7;
        public override int SpriteWidth => 20;
        public override int SpriteHeight => 24;
    }

    public class WeightGainPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<WeightGainPotion>().Texture;
        public override int PotionDamageModifier => 3;
        public override float PotionWeightModifier => 2f;
        public override int PotionExplosionRadius => 8;
        public override int SpriteWidth => 30;
        public override int SpriteHeight => 24;
    }

    public class GreaterWeightGainPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<GreaterWeightGainPotion>().Texture;
        public override int PotionDamageModifier => 6;
        public override float PotionWeightModifier => 3f;
        public override int PotionExplosionRadius => 9;
        public override int SpriteWidth => 32;
        public override int SpriteHeight => 32;
    }

    public class SuperWeightGainPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<SuperWeightGainPotion>().Texture;
        public override int PotionDamageModifier => 11;
        public override float PotionWeightModifier => 4f;
        public override int PotionExplosionRadius => 10;
        public override int SpriteWidth => 64;
        public override int SpriteHeight => 58;
    }

    public class LesserWeightLossPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<LesserWeightLossPotion>().Texture;
        public override int PotionDamageModifier => 1;
        public override float PotionWeightModifier => -1f;
        public override int PotionExplosionRadius => 7;
        public override int SpriteWidth => 16;
        public override int SpriteHeight => 26;
    }

    public class WeightLossPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<WeightLossPotion>().Texture;
        public override int PotionDamageModifier => 3;
        public override float PotionWeightModifier => -2f;
        public override int PotionExplosionRadius => 8;
        public override int SpriteWidth => 16;
        public override int SpriteHeight => 24;
    }

    public class GreaterWeightLossPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<GreaterWeightLossPotion>().Texture;
        public override int PotionDamageModifier => 6;
        public override float PotionWeightModifier => -3f;
        public override int PotionExplosionRadius => 9;
        public override int SpriteWidth => 18;
        public override int SpriteHeight => 30;
    }

    public class SuperWeightLossPotionProjectile : WeightPotionProjectile
    {
        public override string Texture => ModContent.GetInstance<SuperWeightLossPotion>().Texture;
        public override int PotionDamageModifier => 11;
        public override float PotionWeightModifier => -4f;
        public override int PotionExplosionRadius => 10;
        public override int SpriteWidth => 22;
        public override int SpriteHeight => 32;
    }
}

