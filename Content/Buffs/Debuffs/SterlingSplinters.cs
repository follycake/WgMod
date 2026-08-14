using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class SterlingSplinters : ModBuff
{

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<SterlingSplintersNPC>()._active = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<SterlingSplintersPlayer>()._active = true;
    }
}

public class SterlingSplintersNPC : GlobalNPC
{
    public bool _active;

    public override void ResetEffects(NPC npc)
    {
        _active = false;
    }

    public override bool InstancePerEntity => true;

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!_active)
            return;

        damage = 5;

        if (npc.lifeRegen > 0)
            npc.lifeRegen = 0;

        npc.lifeRegen -= 20;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!_active)
            return;

        int dustRate = 15;

        if (Main.rand.NextBool(dustRate))
        {
            Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Silver);

            dust.velocity = (npc.Center - dust.position) * 0.05f;
            dust.noGravity = true;
        }
    }
}

public class SterlingSplintersPlayer : ModPlayer
{
    public bool _active;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (!_active || Player != Main.LocalPlayer)
            return;

        if (Player.lifeRegen > 0)
            Player.lifeRegen = 0;

        Player.lifeRegenTime = 0;
        Player.lifeRegen -= 10;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (drawInfo.shadow != 0f || Player.dead || !_active)
            return;

        int dustRate = 15;

        if (Main.rand.NextBool(dustRate))
        {
            Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Silver);

            dust.velocity = (Player.Center - dust.position) * 0.05f;
            dust.noGravity = true;
        }
    }
}
