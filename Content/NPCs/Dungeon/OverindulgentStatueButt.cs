using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace WgMod.Content.NPCs.Dungeon;
public class OverindulgentStatueBottom : ModNPC
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
                int projectileCount = 6;
                int projectileID = ModContent.ProjectileType<HeartyHeart_Wave>();

                if (_style == 1)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Wave_Blue>();
                else if (_style == 2)
                    projectileID = ModContent.ProjectileType<HeartyHeart_Wave_Green>();

                while (projectileCount > 0)
                {
                    projectileCount--;
                    float speedMult = Main.rand.Next(5, 18) / 10f;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + new Vector2(Main.rand.Next(-500,501), speedMult * 500), new Vector2(0, -speedMult * 2.5f), projectileID, 30, 0f, ai0: Main.rand.Next(3, 10) * 16);
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
        Rectangle sourceRectangle = new Rectangle(156, 60 * _style, 78, 60);
        Rectangle headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 78 : 0, 60 * _style, 78, 60);
        Vector2 origin = sourceRectangle.Size() / 2f;

        if (NPC.IsABestiaryIconDummy)
        {
            sourceRectangle = new Rectangle(156, 60 * 2, 78, 60);
            headRectangle = new Rectangle(NPC.frameCounter >= 10 ? 78 : 0, 60 * 2, 78, 60);

            spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 5), headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 5), sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + new Vector2(0, 2) - Main.screenPosition, sourceRectangle, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

        spriteBatch.Draw(_headTexture.Value, NPC.Center + new Vector2(0, 2) - Main.screenPosition, headRectangle, NPC.GetAlpha(Color.White), 0f, origin, 1f, SpriteEffects.None, 0f);
        return false;
    }
}
