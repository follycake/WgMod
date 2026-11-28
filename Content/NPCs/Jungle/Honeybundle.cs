using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Curses;
using WgMod.Content.Projectiles.Enemy;

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

        if (NPC.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
            NPC.StrikeInstantKill();
    }

    public override void OnKill()
    {
        for (int i = 0; i < 360; i += 36)
        {
            Vector2 velocity = new Vector2(0, Main.rand.NextFloat(3f, 7f)).RotatedBy(MathHelper.ToRadians(i)).RotatedByRandom(MathHelper.ToRadians(Main.rand.Next(-25, 26)));

            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, velocity, ModContent.ProjectileType<HoneyGas>(), NPC.damage, 0f);
        }
    }

    public override bool CanHitNPC(NPC target) => false;

    public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
}

