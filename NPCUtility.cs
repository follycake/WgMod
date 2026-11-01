using Terraria;
using Terraria.ModLoader;

namespace WgMod;

public static class NPCUtility
{
    public static bool Exists<T>() where T : ModNPC
    {
        int type = ModContent.NPCType<T>();
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.type == type)
                return true;
        }
        return false;
    }

    static readonly bool[] _preHardmodeBosses =
    [
        NPC.downedBoss1,
        NPC.downedBoss2,
        NPC.downedQueenBee,
        NPC.downedBoss3
    ];

    static readonly bool[] _hardmodeBosses =
    [
        NPC.downedQueenSlime,
        NPC.downedMechBoss1,
        NPC.downedMechBoss2,
        NPC.downedMechBoss3,
        NPC.downedPlantBoss,
        NPC.downedGolemBoss,
        NPC.downedEmpressOfLight,
        NPC.downedAncientCultist
    ];

    // Im sure you can come up with a better name
    public static void ApplyTownNPCModifiers(this NPC npc) // vscode may complain about a using directive being missing, obey it
    {
        float damageMultiplier = 1;
        int lifeMaxModifier = 0;
        int defenseModifier = 0;

        int preHardmodeStats = 0;
        int hardmodeStats = 0;

        if (NPC.combatBookWasUsed)
        {
            damageMultiplier += 0.25f;
            lifeMaxModifier += 250;
            defenseModifier += 8;
        }

        if (NPC.combatBookVolumeTwoWasUsed)
        {
            damageMultiplier += 0.25f;
            lifeMaxModifier += 250;
            defenseModifier += 8;
        }

        foreach (bool item in _preHardmodeBosses)
        {
            if (item)
                preHardmodeStats++;
        }

        if (preHardmodeStats > 0)
        {
            damageMultiplier += 0.1f * preHardmodeStats;
            defenseModifier += 3 * preHardmodeStats;
        }

        foreach (bool item in _hardmodeBosses)
        {
            if (item)
                hardmodeStats++;
        }

        if (hardmodeStats > 0)
        {
            damageMultiplier += 0.15f * hardmodeStats;
            defenseModifier += 8 * hardmodeStats;
        }

        // put ur npc stuff here like this for example
        npc.damage = (int)(npc.damage * damageMultiplier);
        npc.defense += defenseModifier;
        npc.lifeMax += lifeMaxModifier;
    }
}
