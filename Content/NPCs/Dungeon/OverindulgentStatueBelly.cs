using System;
using System.Collections.Generic;
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

namespace WgMod.Content.NPCs.Dungeon;
public class OverindulgentStatueMiddle : ModNPC
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
        NPC.value = 4525;
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
            {
                _style = Main.rand.Next(3);
            }
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
        {
            return;
        }

        NPC.ai[0]++;
        if (NPC.ai[0] > Utility.TimeToTicks(seconds: 8))
        {
            NPC.ai[0] = 0;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                List<double> validAngles =
                    [
                        0,
                        22.5,
                        45,
                        67.5,
                        90,
                        112.5,
                        135,
                        157.5,
                        180,
                        202.5,
                        225,
                        247.5,
                        270,
                        292.5,
                        315,
                        337.5
                    ];
                int projectileCount = 4;
                int projectileID = ModContent.ProjectileType<HeartyHeart_Surround_Strong>(); //yes i probably shouldve made them one projectile but i dont feel like it

                if (_style == 1)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Surround_Blue_Strong>();
                else if (_style == 2)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Surround_Green_Strong>();

                int otherWay = Main.rand.Next(2);

                while (projectileCount > 0)
                {
                    projectileCount--;
                    int pickedIndex = Main.rand.Next(validAngles.Count);
                    double thisAngle = validAngles[pickedIndex];
                    validAngles.RemoveAt(pickedIndex);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, projectileID, 30, 0f, ai0: target.whoAmI, ai1: (float)thisAngle, ai2: otherWay);
                }
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
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, dustID);
        }
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
        NPC.frame.Width = NPC.width;
        NPC.frame.Height = NPC.height;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Rectangle sourceRectangle = new Rectangle(0, 60 * _style, 78, 60);
        Rectangle headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 78 : 0, 60 * _style, 78, 60);
        Vector2 origin = sourceRectangle.Size() / 2f;

        if (NPC.IsABestiaryIconDummy)
        {
            sourceRectangle = new Rectangle(0, 60 * 2, 78, 60);
            headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 78 : 0, 60 * 2, 78, 60);

            spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 5), headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 5), sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }

        spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 2) - Main.screenPosition, headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);

        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 2) - Main.screenPosition, sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
        return false;
    }
}

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
                Main.NewText(Projectile.damage);
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
        {
            _firstTick = false;
        }
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
        {
            pos = 1;
        }
        else if (Projectile.type == ModContent.ProjectileType<HeartyHeart_Surround_Green_Strong>())
        {
            pos = 2;
        }

        Rectangle sourceRectangle = new Rectangle((texture.Width / 3) * pos, 0, texture.Width / 3, texture.Height);
        Vector2 origin = sourceRectangle.Size() / 2f;

        Color drawColor = Projectile.GetAlpha(Color.White);

        for (int k = 1; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + (Projectile.Size / 2) - Main.screenPosition;
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
public class HeartyHeart_Surround_Blue_Strong : HeartyHeart_Surround_Strong;
public class HeartyHeart_Surround_Green_Strong : HeartyHeart_Surround_Strong;
