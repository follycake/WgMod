using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles.Minions;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class HellsBeesBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }
}

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class HellishBee : ModProjectile
{
    public const int StageCount = 4;
    public const int MaxStage = StageCount - 1;

    int _weightProgress;
    int _weightStage;
    int _flyFrame;

    float _speedModifier;
    float _damageModifier;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 8;
        Main.projPet[Type] = true;

        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        ProjectileID.Sets.CultistIsResistantTo[Type] = true;
    }

    public sealed override void SetDefaults()
    {
        Projectile.width = 34;
        Projectile.height = 40;
        Projectile.tileCollide = false;

        Projectile.friendly = true;
        Projectile.minion = true;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.minionSlots = 0f;
        Projectile.penetrate = -1;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int caramelized = ModContent.BuffType<Caramelized>();
        if (target.HasBuff(caramelized) && _weightStage < MaxStage)
        {
            target.DelBuff(target.FindBuffIndex(caramelized));
            _weightProgress++;

            SoundEngine.PlaySound(WgSounds.Gulp, Projectile.Center);

            if (_weightProgress >= 7)
            {
                _weightProgress = 0;
                _weightStage++;
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(WgSounds.Belly, Projectile.Center);
            }
        }
        Projectile.scale = float.Lerp(1f, 1.1f, _weightStage / (float)MaxStage);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        _damageModifier = float.Lerp(1, 2, _weightStage / (float)MaxStage);
        modifiers.SourceDamage *= _damageModifier;
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
        int dustRate = 5;
        if (Main.rand.NextBool(dustRate) && _weightStage == MaxStage)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Honey, 0f, 0.5f, 100, new Color(151, 93, 15), 0.7f);
        }

        Player owner = Main.player[Projectile.owner];
        if (!CheckActive(owner))
            return;
        GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
        SearchForTargets(
            owner,
            out bool foundTarget,
            out float distanceFromTarget,
            out Vector2 targetCenter
        );
        Movement(
            foundTarget,
            distanceFromTarget,
            targetCenter,
            distanceToIdlePosition,
            vectorToIdlePosition
        );
        Visuals();
    }

    bool CheckActive(Player owner)
    {
        if (owner.dead || !owner.active)
        {
            owner.ClearBuff(ModContent.BuffType<HellsBeesBuff>());
            return false;
        }
        if (owner.HasBuff(ModContent.BuffType<HellsBeesBuff>()))
            Projectile.timeLeft = 2;
        return true;
    }

    void GeneralBehavior(
        Player owner,
        out Vector2 vectorToIdlePosition,
        out float distanceToIdlePosition
    )
    {
        Vector2 idlePosition = owner.Center;
        idlePosition.Y -= 48f;

        float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
        idlePosition.X += minionPositionOffsetX;

        vectorToIdlePosition = idlePosition - Projectile.Center;
        distanceToIdlePosition = vectorToIdlePosition.Length();

        if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
        {
            Projectile.position = idlePosition;
            Projectile.velocity *= 0.1f;
            Projectile.netUpdate = true;
        }

        float overlapVelocity = 0.04f;
        foreach (var other in Main.ActiveProjectiles)
        {
            if (
                other.whoAmI != Projectile.whoAmI
                && other.owner == Projectile.owner
                && Math.Abs(Projectile.position.X - other.position.X)
                    + Math.Abs(Projectile.position.Y - other.position.Y)
                    < Projectile.width
            )
            {
                if (Projectile.position.X < other.position.X)
                    Projectile.velocity.X -= overlapVelocity;
                else
                    Projectile.velocity.X += overlapVelocity;

                if (Projectile.position.Y < other.position.Y)
                    Projectile.velocity.Y -= overlapVelocity;
                else
                    Projectile.velocity.Y += overlapVelocity;
            }
        }
    }

    void SearchForTargets(
        Player owner,
        out bool foundTarget,
        out float distanceFromTarget,
        out Vector2 targetCenter
    )
    {
        distanceFromTarget = 700f;
        targetCenter = Projectile.position;
        foundTarget = false;

        if (owner.HasMinionAttackTargetNPC)
        {
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            float between = Vector2.Distance(npc.Center, Projectile.Center);

            if (between < 2000f)
            {
                distanceFromTarget = between;
                targetCenter = npc.Center;
                foundTarget = true;
            }
        }

        if (!foundTarget)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);
                    bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Collision.CanHitLine(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        npc.position,
                        npc.width,
                        npc.height
                    );
                    bool closeThroughWall = between < 100f;

                    if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
        }
        Projectile.friendly = foundTarget;
    }

    void Movement(
        bool foundTarget,
        float distanceFromTarget,
        Vector2 targetCenter,
        float distanceToIdlePosition,
        Vector2 vectorToIdlePosition
    )
    {
        _speedModifier = float.Lerp(0.5f, 1f, _weightStage / (float)MaxStage);

        float speed = 8f / _speedModifier;
        float inertia = 20f / _speedModifier;

        if (foundTarget)
        {
            if (distanceFromTarget > 40f)
            {
                Vector2 direction = targetCenter - Projectile.Center;
                direction.Normalize();
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            }
        }
        else
        {
            if (distanceToIdlePosition > 600f)
            {
                speed = 12f / _speedModifier;
                inertia = 60f / _speedModifier;
            }
            else
            {
                speed = 4f / _speedModifier;
                inertia = 80f / _speedModifier;
            }

            if (distanceToIdlePosition > 20f)
            {
                vectorToIdlePosition.Normalize();
                vectorToIdlePosition *= speed;
                Projectile.velocity =
                    (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
            }
            else if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
        }
    }

    void Visuals()
    {
        Projectile.rotation = Projectile.velocity.X * 0.05f;

        if (Projectile.velocity.X > 0)
            Projectile.spriteDirection = -1;
        else
            Projectile.spriteDirection = 1;

        const int flyFrameSpeed = 5;
        Projectile.frameCounter++;

        if (Projectile.frameCounter >= flyFrameSpeed)
        {
            _flyFrame++;
            _flyFrame %= 2;

            Projectile.frameCounter = 0;
            Projectile.frame = _weightStage * 2 + _flyFrame;
        }

        Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write((byte)_weightStage);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        byte stage = reader.ReadByte();
        if (_weightStage != stage)
            SoundEngine.PlaySound(WgSounds.Belly, Projectile.Center);
        _weightStage = stage;
    }
}

/*
                                                                                                                                                                          :=====:
                                                                                                                                                                   =#############*+==+-
                                                                                                                                                              .*######################*-==
                                                                                                                                                           -#############***#############*=*
                                                                                                                                                        :########+                .########**+
                                                                                                                                                      #######-                        #######*#
                                                                                                                                                   :######.                             ########:
                                                                                                                                                 +#####:                                  #######.
                                                                                                                    :-======::                 +####*                                      #######
                                                                                                             :########################=.     *####=                                         *######
                                                                                                          #####################################%-                                            ######=
                                                                                                       :%##%#####-                    :*#####+                                                ######
                                                                                                      %%#%#%%=                                                                                :#####-
                                                                                                    :%%#%#%:                                                                                   ######
                                                                             .                     .%%%%%*                                                                                     :#####
                                                                     -*=-:-===+++***+              %%#%%=                                                                                       #####-
                                                                 -=.-=+***************#*          +%%%%+                                                                                        *####*
                                                              =-:=+****+:         =*##**##        %%%%%                                                                                         :####*
                                                            =:=+**+-                  *####+      %%%%=                                                         =@           @           *@@    .####*
                                                         :==+***-                       *####     %%%%:                                                    -     @%=%:@      :@  %@  @@@  @+     ####+
                                                       -==***+                            *###    %%%@.                                                      @ *@-@   =@     %@@ @@*            :####:
                                                     -=+***=                               =###   %%%@.                                             @@*+@    @@   @@    *@@@%  *=               =####
                      :==+++=====---::.            -++***=                                  :###*%%%%@:                                    @%=@*   @:  .=*%-  @@   @=           @               ####-
                *#**+**********************==--::=+****@:                                    -#@@%%%%%-                            %@%    @   =@           @%  @:               @              -####
            *##**###*##*#**+++===+***+:.-++*********#%*+=                                     =@@@%%%%*                @-        %=   @   @*@#      @@%@@@@                     @              ####:
         =##########*-       :::::-:: .-+++:        .=**+:                             :*#*    #@@@%%%@:                @:       @::%@     @*-:=#@#                       @@%+*@*             *###+
       =#########+                     :**+-. .:::::-=***=                             -%%#:   .@@@%%%%=        .-==:.   @#@@@@@+-@     :*                                                   -####
      #########:                            .:-:-:-:::=-::-+**-            ..:.       :=:       #@@%%%%@:      @=    .*@:.@.    @+ .+%*:                                                    :####
    :########=                               ::::..   ....:+*+=::-=:     :...    ::=#@@%.        @@@%%%%*      @:      :@ .@@@@%:                                                          .####
   :%######%                             .::::....    .:-==:   :***=             --=+%%%-        #@* :%%@=     .@%:.:=@%                                                                  .%###
   %######%                              :---:.....+#%@@@@@=   :+***:             . .+##=        =#+  :@%@-       . .                                                                    .%##%
  *######%:                              -+*#*:   .*@@@@@@@%**#%****-. .             =##*:       :#+    %%@=                                                                            :%##%
  %######*                           :---=%@@@=    =@@@@@@@@@%%%***#*=---         ...-*##=       :#=     %%%*.                                                                         =###*
 :%#####%:                           .---=*@@@*.   .#@@@@#*=:====*##*+=-:.:=+=    ...:+##+       -#:      -%%@-                                                                       *##%=
 =%#####@                             :--=+%@@@=    .:.      .-====:.     =##*:  ::...=##*:      *#         %%%%: .                                                                 .%##%
 =%#####@                             :-===**+=.          ...-***=.       :-::-*##+:..-=:        ##           %%@*.                                                                =%###
 :%#####@                              =***=:....   .....:-===*##*=:=++===-:..:=+=::-=-.        .#-             @%@*:                                                             %##%:
  %#####@                              -*##*-.:::........:-===*%@@@@@@@+-::::--.   .-=-::--:    *#                %%%%:                                                         =%%#%
  @#####@:                             .+###+===-:...::::-====+%@@@@@@@=.  .-==:.:==:..:-==-    #=                  *%%@-.                                                    :%#%%:
  =%####%#                              =###*====---=========+*@@@@@@@@*. ..=%@@@@@@=...:===.  *#                     -%%@*:.                                               .*%%%*
   @#####@                              :-:...-======-::...+@@@@@@@@%%##=...+@@@@@@@*.  .-==: :#:                        #%%@=.                                            +%%%%
   -%####%*                        ::--=:.   .:===:        :%%*=:.  -*##+:::=%@@@@@@%.   :==-.#*                            %%%@=..                                      =%%%%.
    @#####@:                    -=======-.   ..===:                 .*##*+===*@@@@@@@=   .===##                               .%%%@+:.                                 =%%%%-
     @#####@                    :=-:.         .:-:.              ...:=###*===*@@@@@@@*   :  -#                                    %%%@%-.                            =%%%%-
     :%####%*                 :::          ...:                  ...:=-:  :===%@@@#=:::===. #-                                       +%%%@*:.                     .*%%%%:
      =%####@=              :===:         .:===.              ....=*#+    .-::-===:.:.:-:. #*                                            %%%%@#-.               :%%%%%:
       =%####@+             :-::          .:=+*=          .......:+##+.    ..::-===--=-   #*                                                :%%%%@@+:.       .=@%%%%
        :%####%%           ::          ..:-+*###:    .........::-=-        ...:=###*===. ##                                                      #%%%%@@%**%@%%%%*
          %#####@:         :-.         .-===****++###=:..::==**###+     .......=**=:    ##                                                           -%%%%%%%%%:
           :%#####@         -:         .=****====*#*+==**######**=:    ....::=++:      ##                                                                  :.
             =%####%@       :-.     ...:=###*+::      :*##*=:.        ...:.:=###-     ##
               :%#####@:     -:.   ....:=*###-                      .......:=*##+    ##
                  %%####%#   :-.. ..:=*######+                      ..:::--==*##*. :#*
                    =%#####@*::::-==+*###***+=.                  ....:=======+-   =#=
                       =%#####%%+====-:   :===:              .......::==+**#*:   ##:
                          -%%####%@+      :===-.         ......:.:-=+**+-:      ##
                              *%#####%@+   -===:..........:.:-=***+-.         =##
                                  *%######%@%+======-:.::=***=:              ##=
                                      :%%#######%%#+==-::.                 -##
                                           .+%%#######%%%+:              :##*
                                                  =%%###########%%%%%#%%###.
                                                          :=#%###########=
*/
