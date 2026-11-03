using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

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
        {
            player.buffTime[buffIndex] = 18000;
        }
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
    enum State
    {
        Following,
        Attacking,
    }

    static readonly int _targetingDistance = 2000;
    static readonly int _projectileCooldown = 100;

    int _shootOffset = 0;
    int _baseDamage;
    State _state = State.Following;
    int _attackTimer = 1;

    bool _needsUpdate = false;

    public int Stage
    {
        get => (int)Projectile.ai[0];
        set
        {
            if (Projectile.ai[0] != value)
            {
                Projectile.ai[0] = value;
                _needsUpdate = true;
            }
        }
    }

    public bool QuickTeleport = false;

    int _teleDelay = 0;
    Vector2 _teleTarget = Vector2.Zero;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(_teleTarget.X);
        writer.Write(_teleTarget.Y);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        _teleTarget = new Vector2(x, y);
    }


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
        _baseDamage = Projectile.damage;
        Player owner = Main.player[Projectile.owner];
        if (owner.ownedProjectileCounts[Type] > 1)
            Projectile.Kill();
    }

    public override bool? CanCutTiles()
    {
        return false;
    }

    public override bool MinionContactDamage()
    {
        return Stage == 2 && Projectile.velocity.Y > 0;
    }

    bool isValidTarget(NPC npc, float maxDist)
    {
        if (!npc.CanBeChasedBy())
            return false;

        float distance = Vector2.Distance(npc.Center, new Vector2(Projectile.Center.X, Projectile.Center.Y + _shootOffset));

        if (distance >= maxDist)
            return false;

        if (!Collision.CanHitLine(new Vector2(Projectile.position.X, Projectile.Center.Y + _shootOffset), Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
            return false;

        return true;
    }

    bool FindTarget(int maxDist, out Vector2 targetPosition)
    {
        NPC ownerAttackTarget = Projectile.OwnerMinionAttackTargetNPC;
        if (ownerAttackTarget != null && isValidTarget(ownerAttackTarget, maxDist * 3f))
        {
            targetPosition = ownerAttackTarget.Center;
            return true;
        }

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (!isValidTarget(npc, maxDist))
                continue;

            targetPosition = npc.Center;
            return true;
        }

        targetPosition = Projectile.position;
        return false;
    }

    void FlyingAI(float speedMult, bool hasHostileTarget, Vector2 targetPosition)
    {

        Player player = Main.player[Projectile.owner];
        float maxAllowedDistance = hasHostileTarget ? 1000 : 500;

        if (_state != State.Following && Projectile.Center.Distance(player.Center) > maxAllowedDistance)
            _state = State.Following;

        if (_state == State.Attacking)
            Projectile.tileCollide = false;

        if (hasHostileTarget && _state == State.Attacking)
        {
            Vector2 direction = targetPosition - Projectile.Center;
            float distance = direction.Length();
            direction = direction.SafeNormalize(Vector2.Zero);
            if (distance > 200f)
            {
                direction *= 6f * speedMult;
                Projectile.velocity.X = (Projectile.velocity.X * 40 + direction.X) / (41f);
                Projectile.velocity.Y = (Projectile.velocity.Y * 40 + direction.Y) / (41f);
            }
            else if (distance < 150f)
            {
                direction *= -4f * speedMult;
                Projectile.velocity.X = (Projectile.velocity.X * 40f + direction.X) / 41f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 40f + direction.Y) / 41f;
            }
            else
                Projectile.velocity *= 0.97f;
        }
        else
        {
            if (_state != State.Following && !Collision.CanHitLine(Projectile.Center, 1, 1, Main.player[Projectile.owner].Center, 1, 1))
                _state = State.Following;

            float speed = 6f;

            if (_state == State.Following)
                speed = 15f;

            _attackTimer = 1;

            Vector2 direction = player.Center - Projectile.Center;

            direction.X -= 30f * Main.player[Projectile.owner].direction;
            direction.Y -= 45f;

            float distance = direction.Length();
            if (distance > 200f && speed < 9f)
                speed = 9f;

            speed = (int)(speed * speedMult);

            if (_state == State.Following && distance < 100f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                _state = State.Attacking;

            if (distance > 2000f)
            {
                Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.width / 2);
            }

            if (distance > 10f)
            {
                direction = direction.SafeNormalize(Vector2.Zero);
                if (distance < 50f)
                    speed /= 2f;

                direction *= speed;
                Projectile.velocity = (Projectile.velocity * 20f + direction) / 21f;
            }
            else
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
                Projectile.velocity *= 0.9f;
            }
        }
    }

    void GroundedAI(bool hasHostileTarget)
    {
        Player player = Main.player[Projectile.owner];

        if (!Projectile.CheckForSolidGround())
            Projectile.velocity.Y += 0.25f;

        Projectile.velocity.X *= 0.93f;
        Projectile.velocity.Y *= 0.97f;

        float maxAllowedDistance = hasHostileTarget ? 1000 : 500;

        if (Projectile.owner == Main.myPlayer && Projectile.Center.Distance(player.Center) > maxAllowedDistance && _teleTarget == Vector2.Zero && _teleDelay >= 60)
        {
            if (FindTeleport(out Vector2 dest, player.position))
            {
                _teleTarget = new Vector2(dest.X * 16f - (Projectile.width / 2) + 8f, dest.Y * 16f - Projectile.height);
                Projectile.netUpdate = true;
            }
        }
    }

    bool FindTeleport(out Vector2 dest, Vector2 target, float range = 10f, int clearence = 2)
    {
        target /= 16;
        Vector2 current = Projectile.Center / 16;

        int i = 0;
        bool found = false;

        while (!found && i < 100)
        {
            i++;
            int x = Main.rand.Next((int)(target.X - range), (int)(target.X + range + 1));
            for (int y = Main.rand.Next((int)(target.Y - range), (int)(target.Y + range + 1)); y < target.Y + range; y++)
            {
                if ((y >= current.Y - 1 && y <= current.Y + 1 && x >= current.X - 1 && x <= current.X + 1) || !Main.tile[x, y].HasUnactuatedTile)
                    continue;

                bool valid = true;

                if (Main.tile[x, y - 1].LiquidType == LiquidID.Lava)
                {
                    valid = false;
                }

                if (!valid || (!Main.tileSolid[Main.tile[x, y].TileType]))
                    continue;

                if (Collision.SolidTiles(x - clearence, x + clearence, y - 3 - clearence, y - 1))
                    continue;

                if (valid)
                {
                    dest = new Vector2(x, y);
                    return true;
                }

                break;
            }
        }

        dest = Vector2.Zero;
        return found;
    }

    void FlyingAnim(int start, int len)
    {
        Projectile.frameCounter++;

        int delay = 15;
        if (Projectile.frameCounter >= len * delay)
            Projectile.frameCounter = 0;

        Projectile.frame = Projectile.frameCounter / delay + start;
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        if (Projectile.velocity.X > 0f)
            Projectile.spriteDirection = Projectile.direction = -1;
        else if (Projectile.velocity.X < 0f)
            Projectile.spriteDirection = Projectile.direction = 1;
    }

    void GroundedAnim(int start, int len, bool hasHostileTarget)
    {
        Projectile.rotation = 0;

        if (!hasHostileTarget && Projectile.frameCounter == 0)
        {
            Projectile.frame = start;
            return;
        }

        Projectile.frameCounter++;
        int delay = 15;
        if (Projectile.frameCounter >= len * delay)
            Projectile.frameCounter = 0;
        Projectile.frame = Projectile.frameCounter / delay + start;
    }

    void SpawnParticles()
    {
        if (Main.rand.NextBool(7))
        {
            Dust particle = Main.dust[Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, 2f)];
            particle.velocity *= 0.3f;
            particle.noGravity = true;
            particle.noLight = true;
        }
    }

    void Attack(int coolDown, float projSpeed, bool hasHostileTarget, Vector2 targetPosition)
    {
        if (_attackTimer > 0)
            if (Main.rand.NextBool(3))
                _attackTimer += 2;
            else
                _attackTimer += 1;

        if (_attackTimer > coolDown)
            _attackTimer = 0;

        if (!hasHostileTarget)
            return;

        Vector2 direction = targetPosition - Projectile.Center;

        if (direction.X > 0f)
            Projectile.spriteDirection = Projectile.direction = -1;
        else if (direction.X < 0f)
            Projectile.spriteDirection = Projectile.direction = 1;

        if (_attackTimer != 0f)
            return;

        _attackTimer += 1;
        if (Main.myPlayer == Projectile.owner)
        {
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= projSpeed;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y + _shootOffset), direction, ProjectileID.ImpFireball, Projectile.damage, Projectile.knockBack, Main.myPlayer);
            proj.timeLeft = 300;
            proj.netUpdate = true;
            Projectile.netUpdate = true;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Stage != 2)
            return false;
        if (Projectile.velocity.Y != 0f)
            return false;

        Projectile.velocity = Vector2.Zero;

        SoundEngine.PlaySound(WgSounds.Thump, Projectile.position);
        for (int i = 0; i < 40; i++)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y + Projectile.height), Projectile.width, 0, DustID.Smoke, 0f, 0f, 0, default, 1f);
            dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            dust.velocity.Y *= 0.3f;
        }

        return false;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!CheckActive(owner))
            return;

        bool hasHostileTarget = FindTarget(_targetingDistance, out Vector2 targetPosition);

        _teleDelay++;

        if (_teleTarget != Vector2.Zero)
        {
            TeleportEffects(Projectile.position);
            Projectile.position = _teleTarget;
            TeleportEffects(Projectile.position);

            _teleDelay = 0;
            _teleTarget = Vector2.Zero;
        }

        if (QuickTeleport && Projectile.owner == Main.myPlayer)
        {
            _teleTarget = Main.MouseWorld;

            _teleTarget.X -= Projectile.width / 2;
            _teleTarget.Y -= Projectile.height / 2;

            QuickTeleport = false;
            Projectile.netUpdate = true;
        }
        else
            QuickTeleport = false;

        switch (Stage)
        {
            case 0:
                Projectile.damage = _baseDamage;

                FlyingAI(0.65f, hasHostileTarget, targetPosition);
                FlyingAnim(0, 3);
                Attack(_projectileCooldown, 10, hasHostileTarget, targetPosition);
                break;
            case 1:
                Projectile.damage = _baseDamage * 2;
                Projectile.minionSlots = 2;

                FlyingAI(0.4f, hasHostileTarget, targetPosition);
                FlyingAnim(3, 3);
                Attack(_projectileCooldown + 10, 9, hasHostileTarget, targetPosition);
                break;
            case 2:
                Projectile.damage = _baseDamage * 4;
                _shootOffset = -6;
                Projectile.minion = false;
                Projectile.minionSlots = 0;
                Projectile.sentry = true;
                Projectile.netImportant = true;
                Projectile.tileCollide = true;
                owner.UpdateMaxTurrets();

                DrawOffsetX = -6;

                Projectile.width = 78;
                Projectile.height = 54;

                GroundedAI(hasHostileTarget);
                GroundedAnim(6, 5, hasHostileTarget);
                Attack(_projectileCooldown + 20, 8, hasHostileTarget, targetPosition);
                break;
        }

        SpawnParticles();

        if (_needsUpdate && Projectile.owner == Main.myPlayer)
            Projectile.netUpdate = true;
    }

    void TeleportEffects(Vector2 position)
    {
        SoundEngine.PlaySound(SoundID.Item8, position);
        for (int i = 0; i < 50; i++)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2.5f);
            dust.velocity *= 3f;
            dust.noGravity = true;
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
