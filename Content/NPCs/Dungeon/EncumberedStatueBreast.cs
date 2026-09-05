using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;
using WgMod.Content.Dusts;

namespace WgMod.Content.NPCs.Dungeon;

public class EncumberedStatueTop : ModNPC
{
    public override string Texture => "WgMod/Content/NPCs/Dungeon/EncumberedStatueSheet";

    const string HeadPath = "WgMod/Content/NPCs/Dungeon/EncumberedStatueHeadSheet";

    static Asset<Texture2D> _headTexture;

    public override void Load()
    {
        _headTexture = ModContent.Request<Texture2D>(HeadPath);
    }

    int _style = -1;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.NeedsExpertScaling[NPC.type] = true;
        NPCID.Sets.CantTakeLunchMoney[NPC.type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[NPC.type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 48;
        NPC.lifeMax = 200;
        NPC.defense = 12;
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.Tink;
        NPC.DeathSound = SoundID.Item127;
        NPC.value = 280;

        Banner = ModContent.NPCType<EncumberedStatueMiddle>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.EncumberedStatue")
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.position.X = (int)(NPC.position.X / 16) * 16 + (Main.rand.NextBool(2) ? 1 : -1); //snap to tile grid
        NPC.netUpdate = true; //im not sure if npc update runs after this or not
    }

    public override void AI()
    {
        if (_style == -1)
        {
            Vector2 bottomPosition = NPC.Center + new Vector2(0, NPC.height / 2 + 4);
            Tile onTileLeft = Framing.GetTileSafely(bottomPosition - new Vector2(4, 0));
            if (onTileLeft.HasTile)
            {
                if (onTileLeft.TileType == TileID.PinkDungeonBrick)
                    _style = 0;
                else if (onTileLeft.TileType == TileID.BlueDungeonBrick)
                    _style = 1;
                else if (onTileLeft.TileType == TileID.GreenDungeonBrick)
                    _style = 2;
            }

            Tile onTileRight = Framing.GetTileSafely(bottomPosition + new Vector2(4, 0));
            if (onTileRight.HasTile)
            {
                if (onTileRight.TileType == TileID.PinkDungeonBrick)
                    _style = 0;
                else if (onTileRight.TileType == TileID.BlueDungeonBrick)
                    _style = 1;
                else if (onTileRight.TileType == TileID.GreenDungeonBrick)
                    _style = 2;
            }

            if (_style == -1)
                _style = Main.rand.Next(3);
        }

        switch (_style)
        {
            case 0:
                Lighting.AddLight(NPC.Center - new Vector2(0, 4), new Vector3(255, 51, 160) / 255 * 0.9f);
                break;
            case 1:
                Lighting.AddLight(NPC.Center - new Vector2(0, 4), new Vector3(51, 109, 255) / 255 * 0.9f);
                break;
            case 2:
                Lighting.AddLight(NPC.Center - new Vector2(0, 4), new Vector3(51, 255, 92) / 255 * 0.9f);
                break;
        }

        NPC.velocity.X *= 0.8f;
        if (NPC.velocity.X > -0.05f && NPC.velocity.X < 0.05f)
            NPC.velocity.X = 0;
        NPC.TargetClosest(false);
        if (!NPC.HasValidTarget)
            return;
        Player target = Main.player[NPC.target];

        if (target.Center.Distance(NPC.Center) > 950)
            return;

        NPC.ai[0]++;
        if (NPC.ai[0] > Utility.TimeToTicks(seconds: 8))
        {
            NPC.ai[0] = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float thisAngle = MathHelper.ToRadians(Main.rand.Next(0, 360));
                int projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Spawner>();
                Vector2 spawnHere = target.Center + new Vector2(Main.rand.Next(150, 226), 0).RotatedBy(thisAngle);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnHere, Vector2.Zero, projectileID, 0, 0f,
                    ai0: target.whoAmI, ai1: _style);
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        short dustID = DustID.DungeonPink;

        if (_style == 1)
            dustID = DustID.DungeonBlue;
        else if (_style == 2)
            dustID = DustID.DungeonGreen;

        int num = NPC.life > 0 ? 3 : 12;
        num += (int)Math.Clamp(hit.Damage / 10f, 0, 5);

        for (int k = 0; k < num; k++)
            Dust.NewDust(NPC.position, NPC.width, NPC.height, dustID);
    }

    public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        if (item.pick >= 100) //pickaxe damage boost
        {
            modifiers.Defense *= 0f;
            modifiers.FinalDamage *= 4f;
        }
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (NPC.FindFirstNPC(NPC.type) > 0) //prevent multiple of the same
            return 0f;
        float chance = SpawnCondition.DungeonNormal.Chance * 0.03f;
        if (NPC.downedPlantBoss)
            chance /= 3f;
        int onTile = spawnInfo.SpawnTileType;
        if (onTile == TileID.BlueDungeonBrick || onTile == TileID.PinkDungeonBrick ||
            onTile == TileID.GreenDungeonBrick)
            return chance;
        return 0f;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter >= 20)
            NPC.frameCounter = 0;
        NPC.frame = new Rectangle(58, 0, 58, 58);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Rectangle sourceRectangle = new(58, 58 * _style, 58, 58);
        Rectangle headRectangle = new(NPC.frameCounter >= 10 ? 58 : 0, 58 * _style, 58, 58);
        Vector2 origin = sourceRectangle.Size() / 2f;

        if (NPC.IsABestiaryIconDummy)
        {
            sourceRectangle = new Rectangle(58, 58 * 2, 58, 58);
            headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 58 : 0, 58 * 2, 58, 58);

            spriteBatch.Draw(_headTexture.Value, NPC.Center, headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f,
                SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center, sourceRectangle, drawColor, 0f, origin, 1f,
                SpriteEffects.None, 0f);
            return false;
        }

        spriteBatch.Draw(_headTexture.Value, NPC.Center - new Vector2(0, 3) - Main.screenPosition, headRectangle,
            NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);

        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - new Vector2(0, 3) - Main.screenPosition,
            sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        return false;
    }
}

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
        target.AddBuff(ModContent.BuffType<Infatuated>(), 600, false);
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
