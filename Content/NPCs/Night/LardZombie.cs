using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Systems;
using WgMod.Content.Items.Curses;
using WgMod.Content.Items.Placeable.Banners;

namespace WgMod.Content.NPCs.Night;

public class LardZombie : ModNPC
{
    int _spawnedLardlings = 0;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Position = new Vector2(0, 24),
            PortraitPositionYOverride = 0f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    public override void SetDefaults()
    {
        NPC.width = 140;
        NPC.height = 70;
        NPC.damage = 12;
        NPC.defense = 33;
        NPC.lifeMax = 1425;
        NPC.value = 3375f;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 3f;

        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;

        if (NPC.downedPlantBoss && Main.expertMode)
        {
            NPC.defense = 36;
            NPC.lifeMax = 1600;
        }

        NPC.rarity = 1;

        Banner = Type;
        BannerItem = ModContent.ItemType<LardZombieBanner>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
            new FlavorTextBestiaryInfoElement(Mod.GetLocalizationKey("Bestiary.LardZombie"))
        ]);
    }

    public override void OnSpawn(IEntitySource source)
    {
        NPC.direction = Main.rand.NextBool() ? 1 : -1;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Main.hardMode)
            return 0f;

        if (NPC.FindFirstNPC(Type) >= 0)
            return 0f;

        float chance = SpawnCondition.OverworldNightMonster.Chance * 0.01f;

        if (Main.bloodMoon)
            chance *= 5f;

        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;

        return chance;
    }

    public void SpawnLardling()
    {
        SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(0.1f), NPC.Center);

        DumbParticleSystem.LardExplode(NPC.GetSource_FromAI(), 12, new Vector2(NPC.Center.X, NPC.position.Y + 8), 3f, false, true);

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC evilThing = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.position.Y + 8, ModContent.NPCType<Lardling>());
            evilThing.velocity = new Vector2(Main.rand.NextFloat(3f, 4f) * NPC.direction, Main.rand.NextFloat(-2f, 2f));
        }
    }

    public override void AI()
    {
        NPC.spriteDirection = NPC.direction;
        NPC.velocity.X *= 0.85f;

        if (NPC.velocity.Y < -0.5f)
            NPC.velocity.Y *= 0.9f;

        if (NPC.ai[1] < 10)
        {
            NPC.ai[0]++;
            if (NPC.ai[0] > 600)
            {
                NPC.ai[0] = 0;
                NPC.ai[1]++;
                SpawnLardling();
            }
        }

        if (NPC.GetLifePercent() <= 0.8f - (0.2f * _spawnedLardlings) && NPC.life > 0)
        {
            _spawnedLardlings++;
            SpawnLardling();
        }

        NPC.PushAwayFromMe();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        IEntitySource source = NPC.GetSource_Death();

        if (NPC.life <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
        {
            for (int i = 0; i < 3; i++)
            {
                NPC evilThing = NPC.NewNPCDirect(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Lardling>());
                evilThing.velocity = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f));
            }
        }


        if (Main.netMode == NetmodeID.Server)
            return;

        int counter;

        if (NPC.life > 0)
        {   // Why is this code in every new NPC added? Please don't just copy/paste the same thing over and over again, make something more efficient.
            int loops = 0;
            while (loops < hit.Damage / (double)NPC.lifeMax * 100.0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, NPC.alpha, Scale: NPC.scale);
                counter = loops;
                loops = counter + 1;
            }
            return;
        }
        for (int i = 0; i < 75; i = counter + 1)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -2f, NPC.alpha, Scale: NPC.scale);
            counter = i;
        }

        Gore.NewGore(source, NPC.Top, NPC.velocity + new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-2f, -1f)), 262, 1f);

        DumbParticleSystem.LardExplode(source, 120, NPC.Hitbox, 7f, true, true);
        return;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool CanHitNPC(NPC target)
    {
        return false;
    }
}
