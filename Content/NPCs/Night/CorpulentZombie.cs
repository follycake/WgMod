using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Players;
using WgMod.Common.Systems;
using WgMod.Content.Items.Curses;

namespace WgMod.Content.NPCs.Night;

public class CorpulentZombie : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

        NPCID.Sets.ShimmerTransformToNPC[Type] = NPCID.Skeleton;

        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Velocity = 1f,
            Position = new Vector2(0, 10),
            PortraitPositionYOverride = 0f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    public override void SetDefaults()
    {
        NPC.width = 70;
        NPC.height = 60;
        NPC.damage = 20;
        NPC.defense = 8;
        NPC.lifeMax = 185;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 210f;
        NPC.knockBackResist = 0.3f;
        NPC.aiStyle = NPCAIStyleID.Fighter;

        if (NPC.downedPlantBoss && Main.expertMode)
        {
            NPC.defense = 16;
            NPC.lifeMax = 255;
            NPC.damage = 46;
        }

        AIType = NPCID.Zombie;
        AnimationType = NPCID.Zombie;

        Banner = Item.NPCtoBanner(NPCID.Zombie);
        BannerItem = Item.BannerToItem(Banner);
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
            new FlavorTextBestiaryInfoElement(Mod.GetLocalizationKey("Bestiary.CorpulentZombie"))
        ]);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!NPC.downedBoss1 && !NPC.downedBoss2 && !NPC.downedBoss3 && !Main.hardMode && !Main.getGoodWorld)
            return 0f;
        float chance = SpawnCondition.OverworldNightMonster.Chance * 0.11f;

        if (Main.bloodMoon)
            chance *= 2f;

        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;

        return chance;
    }

    public override void AI()
    {
        NPC.velocity.X *= 0.85f;

        if (NPC.velocity.Y < -0.5f)
            NPC.velocity.Y *= 0.9f;

        NPC.PushAwayFromMe();
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
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
        for (int i = 0; i < 50; i = counter + 1)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -2f, NPC.alpha, Scale: NPC.scale);
            counter = i;
        }

        IEntitySource source = NPC.GetSource_Death();

        Gore.NewGore(source, NPC.Top, NPC.velocity + new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-2f, -1f)), 262, 1f);

        DumbParticleSystem.LardExplode(source, 50, NPC.Hitbox, 5f, true, true);
        return;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 4f + hurtInfo.Damage / 10f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50));
        npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250));
    }

}
