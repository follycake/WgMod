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
    public override string Texture => "WgMod/Content/Projectiles/Enemy/Honeygas";
    int _style = -1;

    public static float Easing(float t)
    {
        return 1 - (float)Math.Pow(1 - t, 5);
    }
    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = true;
    }
    public override void AI()
    {
        float maxTime = 200f;

        Projectile.ai[0]++;

        if (Projectile.ai[0] > maxTime)
        {
            Projectile.Kill();
            Projectile.ai[0] = maxTime;
            return;
        }

        Projectile.Opacity = 1 - (Projectile.ai[0] / maxTime);
        Projectile.scale = float.Lerp(0.25f, 1.5f, Easing(Projectile.ai[0] / maxTime));

        Projectile.velocity *= 0.96f;

        Projectile.rotation += Projectile.velocity.Length() / 15f;

        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 pos = Projectile.Center + dir * 16f;
        if (Collision.IsWorldPointSolid(pos, true))
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity - dir * 1f, 0.5f);
        }
    }
    public override bool CanHitPlayer(Player target)
    {
        return Projectile.ai[0] < 110f;
    }
    public override bool? CanHitNPC(NPC target)
    {
        return Projectile.ai[0] < 110f ? null : false;
    }
    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.Knockback *= 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(ModContent.BuffType<Honeyspored>(), Utility.TimeToTicks(seconds: 25) + info.Damage * 25, false);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (_style == -1)
            _style = Main.rand.Next(3);

        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Color color = Projectile.GetAlpha(lightColor).MultiplyRGBA(new Color(0.8f, 0.8f, 0.8f, 0.8f));
        Color color2 = color.MultiplyRGBA(new Color(0.4f, 0.4f, 0.4f, 0.4f));

        Rectangle source = new Rectangle(texture.Width / 3 * _style, 0, texture.Width / 3, texture.Height);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color2, Projectile.rotation, source.Size() / 2, Projectile.scale * 2.5f, SpriteEffects.None, 0);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color, Projectile.rotation, source.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

        return false;
    }
}

