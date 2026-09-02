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
using WgMod.Content.Dusts;
using WgMod.Content.Items.Placeable.Banners;

namespace WgMod.Content.NPCs.Dungeon;

public class OverindulgentStatueTop : ModNPC
{
    public override string Texture => "WgMod/Content/NPCs/Dungeon/OverindulgentStatueSheet";

    const string HeadPath = "WgMod/Content/NPCs/Dungeon/OverindulgentStatueHeadSheet";

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
        NPC.height = 64;
        NPC.lifeMax = 1250;
        NPC.defense = 30;
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.Tink;
        NPC.DeathSound = SoundID.Item127;
        NPC.value = 3225;

        Banner = ModContent.NPCType<OverindulgentStatueMiddle>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.OverindulgentStatue")
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
                int projectileID = ModContent.ProjectileType<HeartyHeart_Direct_Spawner_Strong>();
                Vector2 spawnHere = target.Center + new Vector2(Main.rand.Next(175, 251), 0).RotatedBy(thisAngle);

                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnHere, Vector2.Zero, projectileID, 0, 0f, ai0: target.whoAmI, ai1: _style);
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
        if (!NPC.downedPlantBoss)
            return 0f;
        float chance = SpawnCondition.DungeonNormal.Chance * 0.03f;
        int onTile = spawnInfo.SpawnTileType;
        if (onTile == TileID.BlueDungeonBrick || onTile == TileID.PinkDungeonBrick || onTile == TileID.GreenDungeonBrick)
            return chance;
        return 0f;
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter >= 20)
            NPC.frameCounter = 0;
        NPC.frame = new Rectangle(78, 0, 78, 60);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Rectangle sourceRectangle = new(78, 60 * _style, 78, 60);
        Rectangle headRectangle = new(NPC.frameCounter >= 10 ? 78 : 0, 60 * _style, 78, 60);
        Vector2 origin = sourceRectangle.Size() / 2f;

        if (NPC.IsABestiaryIconDummy)
        {
            sourceRectangle = new Rectangle(78, 60 * 2, 78, 60);
            headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 78 : 0, 60 * 2, 78, 60);

            spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 7), headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 7), sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 4) - Main.screenPosition, headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);

        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 4) - Main.screenPosition, sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        return false;
    }
}

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
                float currentAngle = -baseAngle * (projectileCount / 2f) + (baseAngle / 2f);
                Vector2 velocity = new Vector2(2.25f, 0).RotatedBy(Projectile.Center.AngleTo(target.Center));

                for (int projectiles = 0; projectiles < projectileCount; projectiles++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, velocity.RotatedBy(currentAngle), projectileID, 30, 0f);
                    currentAngle += baseAngle;
                }
            }
        }
        else
        {
            Projectile.Kill();
        }
    }
}
