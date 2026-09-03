using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class PillarWrath : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.LongerExpertDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<PillarWrathPlayer>().PillarWrath = true;
    }
}

public class PillarWrathNPC : GlobalNPC
{
    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!npc.HasBuff<PillarWrath>())
            return;
        damage = 50;
        if (npc.lifeRegen > 0)
            npc.lifeRegen = 0;
        npc.lifeRegen -= 200;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!npc.HasBuff<PillarWrath>())
            return;

        int dustRate = 15;
        if (Main.rand.NextBool(dustRate))
        {
            int vortex = Dust.NewDust(
                npc.position,
                npc.width,
                npc.height,
                DustID.Vortex,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[vortex].noGravity = true;
        }

        if (Main.rand.NextBool(dustRate))
        {
            int stardust = Dust.NewDust(
                npc.position,
                npc.width,
                npc.height,
                DustID.Electric,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[stardust].noGravity = true;
        }

        if (Main.rand.NextBool(dustRate))
        {
            int solar = Dust.NewDust(
                npc.position,
                npc.width,
                npc.height,
                DustID.SolarFlare,
                0f,
                0.25f,
                100,
                default,
                1f
            );
        }

        if (Main.rand.NextBool(dustRate))
        {
            int nebula = Dust.NewDust(
                npc.position,
                npc.width,
                npc.height,
                DustID.CrystalPulse,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[nebula].noGravity = true;
            Main.dust[nebula].velocity = new Vector2(0, 0);
        }
    }
}

public class PillarWrathPlayer : ModPlayer
{
    public bool PillarWrath;

    public override void ResetEffects()
    {
        PillarWrath = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (!PillarWrath)
            return;

        if (Player.lifeRegen > 0)
            Player.lifeRegen = 0;

        Player.lifeRegenTime = 0;
        Player.lifeRegen -= 200;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!PillarWrath)
            return;

        int dustRate = 15;

        if (Main.rand.NextBool(dustRate))
        {
            int vortex = Dust.NewDust(
                Player.position,
                Player.width,
                Player.height,
                DustID.Vortex,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[vortex].noGravity = true;
        }

        if (Main.rand.NextBool(dustRate))
        {
            int stardust = Dust.NewDust(
                Player.position,
                Player.width,
                Player.height,
                DustID.Electric,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[stardust].noGravity = true;
        }

        if (Main.rand.NextBool(dustRate))
        {
            int solar = Dust.NewDust(
                Player.position,
                Player.width,
                Player.height,
                DustID.SolarFlare,
                0f,
                0.25f,
                100,
                default,
                1f
            );
        }

        if (Main.rand.NextBool(dustRate))
        {
            int nebula = Dust.NewDust(
                Player.position,
                Player.width,
                Player.height,
                DustID.CrystalPulse,
                0f,
                0f,
                100,
                default,
                1f
            );

            Main.dust[nebula].noGravity = true;
            Main.dust[nebula].velocity = new Vector2(0, 0);
        }
    }
}
