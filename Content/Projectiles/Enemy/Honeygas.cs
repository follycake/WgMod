using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Projectiles.Enemy;

public class HoneyGas : ModProjectile
{
    const float MaxTime = 200;

    float _lifetime = 0;
    int _scale = 0;

    public static float Easing(float t) => 1 - (float)Math.Pow(1 - t, 5);

    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
        _scale = Main.rand.Next(3);
    }

    public override void AI()
    {
        _lifetime++;

        if (_lifetime > MaxTime)
        {
            Projectile.Kill();
            return;
        }

        Projectile.Opacity = 1 - (_lifetime / MaxTime);
        Projectile.scale = float.Lerp(0.25f, 1.5f, Easing(_lifetime / MaxTime));

        Projectile.velocity *= 0.96f;

        Projectile.rotation += Projectile.velocity.Length() / 15f;

        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 pos = Projectile.Center + dir * 16f;
        if (Collision.IsWorldPointSolid(pos, true))
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity - dir * 1f, 0.5f);
        }
    }

    public override bool CanHitPlayer(Player target) => _lifetime < 110f;

    public override bool? CanHitNPC(NPC target) => _lifetime < 110f ? null : false;

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.Knockback *= 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Honeyspored>(), Utility.TimeToTicks(seconds: 5) + info.Damage * 15, false);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Color color = Projectile.GetAlpha(lightColor).MultiplyRGBA(new Color(0.8f, 0.8f, 0.8f, 0.8f));
        Color color2 = color.MultiplyRGBA(new Color(0.4f, 0.4f, 0.4f, 0.4f));

        Rectangle source = new(texture.Width / 3 * _scale, 0, texture.Width / 3, texture.Height);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color2, Projectile.rotation * 0.5f, source.Size() / 2, Projectile.scale * 2.5f, SpriteEffects.None, 0);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color, Projectile.rotation * 1.5f, source.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

        return false;
    }
}

