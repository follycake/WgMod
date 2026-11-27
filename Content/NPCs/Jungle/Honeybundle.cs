using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;
using WgMod.Content.Items.Curses;

namespace WgMod.Content.NPCs.Jungle;

public class Honeybundle : ModNPC
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;
        NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
        NPCID.Sets.NeverDropsResourcePickups[NPC.type] = true;

        var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifier);
    }

    public override void SetDefaults()
    {
        NPC.width = 16;
        NPC.height = 16;
        NPC.lifeMax = 1;
        NPC.HideStrikeDamage = true;
        NPC.knockBackResist = 0f;
        NPC.damage = 5;
        NPC.ShowNameOnHover = false;
        NPC.npcSlots = 0.1f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.position.X = (int)(NPC.position.X / 16) * 16 + (Main.rand.NextBool(2) ? 1 : -1); //snap to tile grid
        NPC.netUpdate = true; //im not sure if npc update runs after this or not
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.SpawnTileType != TileID.JungleGrass)
            return 0f;

        float chance = 0.1f;

        if (spawnInfo.SpawnTileY > (Main.worldSurface + Main.rockLayer) / 2)
            chance = 0.3f;

        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;

        return chance;
    }

    public override void AI()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        Player target = Main.LocalPlayer;
        if (NPC.Hitbox.Intersects(target.Hitbox))
            NPC.StrikeInstantKill();
    }

    public override void OnKill()
    {
        for (int i = 0; i < 360; i += 36)
        {
            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.NextFloat(3f, 7f)).RotatedBy(MathHelper.ToRadians(i)).RotatedByRandom(MathHelper.ToRadians(Main.rand.Next(-25, 26))),
                ModContent.ProjectileType<HoneyGas>(), NPC.damage, 0f);
        }
    }

    public override bool CanHitNPC(NPC target)
    {
        return false;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
}

public class HoneyGas : ModProjectile   // Move this to where it belongs.
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
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity - dir * 1f, 0.5f);
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

        Rectangle source = new(texture.Width / 3 * _style, 0, texture.Width / 3, texture.Height);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color2, Projectile.rotation, source.Size() / 2, Projectile.scale * 2.5f, SpriteEffects.None, 0);

        Main.EntitySpriteDraw(texture, drawPos, source,
            color, Projectile.rotation, source.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

        return false;
    }
}
