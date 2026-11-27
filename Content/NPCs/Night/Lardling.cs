using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Players;
using WgMod.Common.Systems;
using WgMod.Content.Items.Curses;
using WgMod.Content.Items.Placeable.Banners;

namespace WgMod.Content.NPCs.Night;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class Lardling : ModNPC
{
    public override void SetDefaults()
    {
        NPC.width = 18;
        NPC.height = 18;
        NPC.noGravity = true;
        NPC.value = 340;
        NPC.lifeMax = 100;
        NPC.npcSlots = 0.5f;
        NPC.damage = 38;
        NPC.defense = 16;

        NPC.HitSound = SoundID.NPCHit13;
        NPC.DeathSound = SoundID.NPCDeath21;

        if (NPC.downedPlantBoss && Main.expertMode)
        {
            NPC.defense = 18;
            NPC.lifeMax = 120;
            NPC.damage = 48;
        }

        Banner = Type;
        BannerItem = ModContent.ItemType<LardlingBanner>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Lardling")
        ]);
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Main.hardMode)
            return 0f;

        float chance = SpawnCondition.OverworldNightMonster.Chance * 0.03f;

        if (Main.bloodMoon)
            chance *= 3f;

        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;

        return chance;
    }

    public override void AI()
    {
        if (NPC.collideX)
        {
            NPC.velocity.X = NPC.oldVelocity.X * -1;
            if (Math.Abs(NPC.velocity.X) < 3)
                if (NPC.velocity.X > 0)
                    NPC.velocity.X = 3;
                else if (NPC.velocity.X < 0)
                    NPC.velocity.X = -3;
        }

        if (NPC.collideY)
        {
            NPC.velocity.Y = NPC.oldVelocity.Y * -1;
            if (Math.Abs(NPC.velocity.Y) < 3)
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y = 3;
                else if (NPC.velocity.Y < 0)
                    NPC.velocity.Y = -3;
        }

        NPC.TargetClosest(false);

        Vector2 direction;

        if (NPC.HasValidTarget)
        {
            direction = NPC.Center.DirectionTo(Main.player[NPC.target].Center);
            if (Main.IsItDay() && NPC.extraValue <= 0)
            {
                NPC.EncourageDespawn(30);
                direction = Main.player[NPC.target].Center.DirectionTo(NPC.Center);
            }
        }
        else
            direction = NPC.velocity;

        direction.Normalize();

        NPC.velocity *= 0.994f;
        NPC.velocity += direction / 8f;

        float fullSpeed = NPC.velocity.Length();
        if (fullSpeed > 9f)
        {
            NPC.velocity.Normalize();
            NPC.velocity *= 9f;
        }

        NPC.rotation = NPC.velocity.ToRotation();
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;

        if (NPC.frameCounter >= 16)
            NPC.frameCounter = 0;

        int currentFrame = (int)Math.Floor(NPC.frameCounter / 8);

        NPC.frame = new Rectangle(0, 24 * currentFrame, 32, 24);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 origin = NPC.frame.Size() / 2f;

        if (NPC.IsABestiaryIconDummy)
        {
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center, NPC.frame, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }

        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, origin, 1f, SpriteEffects.FlipHorizontally, 0f);

        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        if (NPC.life > 0)
        {
            DumbParticleSystem.LardExplode(NPC.GetSource_FromThis(), (int)(hit.Damage / (double)NPC.lifeMax * 25.0), NPC.Center, 2f, false, true);
            return;
        }

        DumbParticleSystem.LardExplode(NPC.GetSource_Death(), 30, NPC.Center, 3f, true, true);
        return;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;

        Mass weightGain = 3f + hurtInfo.Damage / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);

        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }

}
