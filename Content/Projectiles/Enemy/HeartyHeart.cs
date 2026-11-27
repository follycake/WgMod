using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;
using WgMod.Content.Dusts;

namespace WgMod.Content.Projectiles.Enemy;

public class HeartyHeart_Surround : ModProjectile
{
    public override string Texture => "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBig";

    const string TrailTexture = "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBigTrail";

    static Asset<Texture2D> _trailTexture;

    public override void Load()
    {
        _trailTexture = ModContent.Request<Texture2D>(TrailTexture);
    }

    int _timer = Utility.TimeToTicks(seconds: 1, ticks: 30);
    bool _charging = false;
    bool _firstTick = true;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    public override void AI() //ai0 = target player, ai1 = spawn angle
    {
        if (!_charging)
        {
            Projectile.alpha = (int)Math.Clamp((_timer - 60) / 30f * 255f, 0, 255);
            _timer--;

            Player target = Main.player[(int)Projectile.ai[0]];
            if (!target.active || target.dead)
            {
                Projectile.Kill();
                return;
            }
            if (_timer > 6)
            {
                Projectile.Center = target.Center + new Vector2(175, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));
                Projectile.rotation = MathHelper.ToRadians(Projectile.ai[1]) + MathHelper.PiOver2;
            }

            if (_timer == 0)
            {
                _charging = true;
                SoundEngine.PlaySound(SoundID.Item28.WithPitchOffset(1.2f), Projectile.Center);
            }
            if (_firstTick)
                SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
        }
        else
        {
            _timer++;
            Projectile.velocity = new Vector2(-10, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            Projectile.alpha = (int)Math.Clamp((_timer - 15) / 14f * 255f, 0, 255);

            if (_timer > 30)
                Projectile.Kill();
        }
        if (_firstTick)
            _firstTick = false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return _charging && _timer <= 20;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (_charging && _timer <= 20)
            return null;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int pos = 0;
        if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Surround_Blue>())
            pos = 1;
        else if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Surround_Green>())
            pos = 2;

        Rectangle sourceRectangle = new(texture.Width / 3 * pos, 0, texture.Width / 3, texture.Height);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Color drawColor = Projectile.GetAlpha(Color.White);

        for (int k = 1; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition;
            Color color = drawColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(_trailTexture.Value, drawPos, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        Main.EntitySpriteDraw(texture,
            Projectile.Center - Main.screenPosition,
            sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Infatuated>(), 900, false);
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 8f + (int)Math.Round(info.Damage / 2f) / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }
}
public class HeartyHeart_Surround_Blue : HeartyHeart_Surround;
public class HeartyHeart_Surround_Green : HeartyHeart_Surround;

public class HeartyHeart_Direct : ModProjectile
{
    public override string Texture => "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBig";

    const string TrailTexture = "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBigTrail";

    static Asset<Texture2D> _trailTexture;

    public override void Load()
    {
        _trailTexture = ModContent.Request<Texture2D>(TrailTexture);
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        Projectile.velocity *= 1.015f;
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int pos = 0;

        if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Direct_Blue>())
            pos = 1;
        else if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Direct_Green>())
            pos = 2;

        Rectangle sourceRectangle = new(texture.Width / 3 * pos, 0, texture.Width / 3, texture.Height);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Color drawColor = Projectile.GetAlpha(Color.White);

        for (int k = 1; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition;
            Color color = drawColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(_trailTexture.Value, drawPos, sourceRectangle, color, Projectile.rotation, origin,
                Projectile.scale, SpriteEffects.None, 0);
        }

        Main.EntitySpriteDraw(texture,
            Projectile.Center - Main.screenPosition,
            sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Infatuated>(), 900, false);
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 8f + (int)Math.Round(info.Damage / 2f) / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }
}
public class HeartyHeart_Direct_Blue : HeartyHeart_Direct;
public class HeartyHeart_Direct_Green : HeartyHeart_Direct;
public class HeartyHeart_Direct_Spawner : ModProjectile
{
    Color GetDustColor()
    {
        Color color = new(255, 51, 160);
        if (Projectile.ai[1] == 1)
            color = new(51, 109, 255);
        else if (Projectile.ai[1] == 2)
            color = new(51, 255, 92);
        return color;
    }

    void SmallDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<OutlinedDustSmall>(), Velocity, 0, GetDustColor(), Scale);
    }

    void BigDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<OutlinedDustBig>(), Velocity, 0, GetDustColor(), Scale);
    }

    int _timer = Utility.TimeToTicks(seconds: 1, ticks: 30);

    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    public override void AI() //ai0 = target player, ai1 = color index
    {
        _timer--;
        Player target = Main.player[(int)Projectile.ai[0]];
        if (!target.active || target.dead)
        {
            Projectile.Kill();
            return;
        }

        if (_timer > 0)
        {
            SmallDust(Projectile.Center, new Vector2(Main.rand.Next(5, 11) / 8f, 0).RotatedByRandom(MathHelper.Pi),
                1.65f);
            SmallDust(Projectile.Center, new Vector2(Main.rand.Next(15, 21) / 8f, 0)
                .RotatedBy(Projectile.Center.AngleTo(target.Center))
                .RotatedByRandom(MathHelper.ToRadians(Main.rand.Next(-15, 16))), 1.75f);
        }
        else if (_timer == 0)
        {
            float degrees = 0;
            while (degrees < 360)
            {
                float power = Main.rand.Next(20, 95) / 10f;
                Vector2 dustVelocity = new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(degrees));
                float scale = 1 + Main.rand.Next(0, 20) / 10f;
                BigDust(Projectile.Center, dustVelocity * power, scale);
                degrees += 22.51f;
            }

            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projectileID = ModContent.ProjectileType<HeartyHeart_Direct>();

                if (Projectile.ai[1] == 1)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Blue>();
                else if (Projectile.ai[1] == 2)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Green>();

                int projectileCount = 3;
                float baseAngle = MathHelper.ToRadians(20);
                float currentAngle = -baseAngle * (projectileCount / 2f) + baseAngle / 2f;
                Vector2 velocity = new Vector2(1.75f, 0).RotatedBy(Projectile.Center.AngleTo(target.Center));

                for (int projectiles = 0; projectiles < projectileCount; projectiles++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                        velocity.RotatedBy(currentAngle), projectileID, 10, 0f);
                    currentAngle += baseAngle;
                }
            }
        }
        else
            Projectile.Kill();
    }
}

public class HeartyHeart_Wave : ModProjectile
{
    public override string Texture => "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBig";

    const string TrailTexture = "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBigTrail";

    static Asset<Texture2D> _trailTexture;

    public override void Load()
    {
        _trailTexture = ModContent.Request<Texture2D>(TrailTexture);
    }

    int _timer = Utility.TimeToTicks(seconds: 7);
    float _defaultX = 0f;
    int _wavyTimer = 0;
    bool _firstTick = true;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    static float Easing(float x)
    {
        if (x > 1f)
            x = 2f - x;
        return x < 0.5f ? 2 * x * x : 1 - (float)Math.Pow(-2 * x + 2, 2) / 2f;
    }

    public override void AI()
    {
        if (_firstTick)
        {
            _defaultX = Projectile.position.X;
            Projectile.velocity.X = 0;
        }

        int wavyStrength = (int)Projectile.ai[0] * 2;
        int wavyTime = (int)Projectile.ai[0] + 100;

        _timer--;

        if (_timer > 60 * 6)
            Projectile.alpha = (int)Math.Clamp((_timer - 60 * 6) / 60f * 255f, 0, 255);
        else if (_timer < 60)
            Projectile.alpha = (int)Math.Clamp(255f - _timer / 60f * 255f, 0, 255);
        else
            Projectile.alpha = 0;

        _wavyTimer++;
        if (_wavyTimer > wavyTime)
            _wavyTimer = 0;
        float offset = MathHelper.Lerp(-wavyStrength / 2, wavyStrength / 2, Easing(_wavyTimer / (float)wavyTime * 2f));
        Projectile.position = new Vector2(_defaultX + offset, Projectile.position.Y);
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

        if (_firstTick)
            _firstTick = false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int pos = 0;
        if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Wave_Blue>())
            pos = 1;
        else if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Wave_Green>())
            pos = 2;

        Rectangle sourceRectangle = new(texture.Width / 3 * pos, 0, texture.Width / 3, texture.Height);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Color drawColor = Projectile.GetAlpha(Color.White);

        for (int k = 1; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition;
            Color color = drawColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(_trailTexture.Value, drawPos, sourceRectangle, color, Projectile.rotation, origin,
                Projectile.scale, SpriteEffects.None, 0);
        }

        Main.EntitySpriteDraw(texture,
            Projectile.Center - Main.screenPosition,
            sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Infatuated>(), 900, false);
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 8f + (int)Math.Round(info.Damage / 2f) / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }

    public override bool CanHitPlayer(Player target)
    {
        return _timer < 60 * 6 && _timer > 60;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (_timer < 60 * 6 && _timer > 60)
            return null;
        return false;
    }
}
public class HeartyHeart_Wave_Blue : HeartyHeart_Wave;
public class HeartyHeart_Wave_Green : HeartyHeart_Wave;

public class HeartyHeart_Surround_Strong : ModProjectile
{
    public override string Texture => "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBig";

    const string TrailTexture = "WgMod/Content/Projectiles/Enemy/EncumberedStatueHeartBigTrail";

    static Asset<Texture2D> _trailTexture;

    public override void Load()
    {
        _trailTexture = ModContent.Request<Texture2D>(TrailTexture);
    }

    int _timer = Utility.TimeToTicks(seconds: 2, ticks: 30);
    bool _charging = false;
    bool _firstTick = true;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 8;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    public override void AI() //ai0 = target player, ai1 = spawn angle
    {
        if (!_charging)
        {
            Projectile.alpha = (int)Math.Clamp((_timer - 120) / 30f * 255f, 0, 255);
            _timer--;

            Player target = Main.player[(int)Projectile.ai[0]];
            if (!target.active || target.dead)
            {
                Projectile.Kill();
                return;
            }
            if (_timer > 6)
            {
                if (Projectile.ai[2] == 0)
                    Projectile.ai[1] += MathHelper.ToRadians(40f);
                else
                    Projectile.ai[1] -= MathHelper.ToRadians(40f);

                Projectile.Center = target.Center + new Vector2(195, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));
                Projectile.rotation = MathHelper.ToRadians(Projectile.ai[1]) + MathHelper.PiOver2;
            }

            if (_timer == 0)
            {
                _charging = true;
                SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);
            }
            if (_firstTick)
                SoundEngine.PlaySound(SoundID.Item15.WithPitchOffset(0.8f), Projectile.Center);
        }
        else
        {
            _timer++;
            Projectile.velocity = new Vector2(-15f, 0).RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            Projectile.alpha = (int)Math.Clamp((_timer - 25) / 10f * 255f, 0, 255);

            if (_timer > 35)
                Projectile.Kill();
        }
        if (_firstTick)
            _firstTick = false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return _charging && _timer <= 30;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (_charging && _timer <= 30)
            return null;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

        int pos = 0;
        if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Surround_Blue_Strong>())
            pos = 1;
        else if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Surround_Green_Strong>())
            pos = 2;

        Rectangle sourceRectangle = new(texture.Width / 3 * pos, 0, texture.Width / 3, texture.Height);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Color drawColor = Projectile.GetAlpha(Color.White);

        for (int k = 1; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition;
            Color color = drawColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(_trailTexture.Value, drawPos, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        Main.EntitySpriteDraw(texture,
            Projectile.Center - Main.screenPosition,
            sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Infatuated>(), 900, false);
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 8f + (int)Math.Round(info.Damage / 2f) / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }
}
public class HeartyHeart_Surround_Blue_Strong : HeartyHeart_Surround_Strong;
public class HeartyHeart_Surround_Green_Strong : HeartyHeart_Surround_Strong;

public class HeartyHeart_Direct_Spawner_Strong : ModProjectile
{
    Color GetDustColor()
    {
        Color color = new(255, 51, 160);
        if (Projectile.ai[1] == 1)
            color = new(51, 109, 255);
        else if (Projectile.ai[1] == 2)
            color = new(51, 255, 92);
        return color;
    }

    void SmallDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<OutlinedDustSmall>(), Velocity, 0, GetDustColor(), Scale);
    }

    void BigDust(Vector2 Position, Vector2 Velocity, float Scale)
    {
        Dust.NewDustPerfect(Position, ModContent.DustType<OutlinedDustBig>(), Velocity, 0, GetDustColor(), Scale);
    }

    int _timer = Utility.TimeToTicks(seconds: 2, ticks: 30);
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }

    public override void AI() //ai0 = target player, ai1 = color index
    {
        _timer--;
        Player target = Main.player[(int)Projectile.ai[0]];
        if (!target.active || target.dead)
        {
            Projectile.Kill();
            return;
        }

        if (_timer > 0)
        {
            SmallDust(Projectile.Center, new Vector2(Main.rand.Next(7, 13) / 8f, 0)
                .RotatedByRandom(MathHelper.Pi), 1.85f);
            SmallDust(Projectile.Center, new Vector2(Main.rand.Next(18, 24) / 8f, 0)
                .RotatedBy(Projectile.Center.AngleTo(target.Center))
                .RotatedByRandom(MathHelper.ToRadians(Main.rand.Next(-15, 16))), 2f);
        }
        else if (_timer == 0)
        {
            float degrees = 0;
            while (degrees < 360)
            {
                float power = Main.rand.Next(20, 105) / 10f;
                Vector2 dustVelocity = new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(degrees));
                float scale = 1.25f + Main.rand.Next(0, 20) / 10f;
                BigDust(Projectile.Center, dustVelocity * power, scale);
                degrees += 22.51f;
            }

            SoundEngine.PlaySound(SoundID.Item28, Projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int projectileID = ModContent.ProjectileType<HeartyHeart_Direct>();

                if (Projectile.ai[1] == 1)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Blue>();
                else if (Projectile.ai[1] == 2)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Green>();

                int projectileCount = 6;
                float baseAngle = MathHelper.ToRadians(18);
                float currentAngle = -baseAngle * (projectileCount / 2f) + baseAngle / 2f;
                Vector2 velocity = new Vector2(2.25f, 0).RotatedBy(Projectile.Center.AngleTo(target.Center));

                for (int projectiles = 0; projectiles < projectileCount; projectiles++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center,
                        velocity.RotatedBy(currentAngle), projectileID, 30, 0f);
                    currentAngle += baseAngle;
                }
            }
        }
        else
            Projectile.Kill();
    }
}
