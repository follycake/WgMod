using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common.Players;

public partial class BuffHitPlayer : ModPlayer
{
    void AddNPCs(HashSet<int> table, string mod, params string[] npcs)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (string npc in npcs)
        {
            if (!foundMod.TryFind(npc, out ModNPC foundNpc))
            {
                Mod.Logger.Warn($"Couldn't find buff '{npc}' for mod '{mod}'");
                continue;
            }
            table.Add(foundNpc.Type);
        }
    }

    public override void Load()
    {
        AddModNPCs();
    }

    void AddBuff(int type, int timeToAdd, Mass weightGain, int chance)
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg) || Main.rand.NextBool(chance))
            return;
        Player.AddBuff(type, timeToAdd);
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, Player.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        if (_slimes.Contains(npc.type))
            AddBuff(_slimesBuff, 60 * 6 + (20 * hurtInfo.Damage), hurtInfo.Damage / 12, 4);

        if (_bees.Contains(npc.type))
            AddBuff(_beesBuff, 60 * 10 + (20 * hurtInfo.Damage), hurtInfo.Damage / 10, 8);

        if (_feeders.Contains(npc.type))
            AddBuff(_feedersBuff, 60 * 3 + (20 * hurtInfo.Damage), hurtInfo.Damage / 8, 6);

        if (npc.type == NPCID.HallowBoss && hurtInfo.Damage < 1250)
            AddBuff(_empressBuff, 4 * hurtInfo.Damage, hurtInfo.Damage / 6, 1);
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        if (_slimeProjectiles.Contains(proj.type))
            AddBuff(_slimesBuff, 60 * 6 + (10 * hurtInfo.Damage), hurtInfo.Damage / 12, 4);

        if (_bloaters.Contains(proj.type))
            AddBuff(_beesBuff, 60 * 10 + (10 * hurtInfo.Damage), hurtInfo.Damage / 10, 8);

        if (_feederProjectiles.Contains(proj.type))
            AddBuff(_feedersBuff, 60 * 3 + (10 * hurtInfo.Damage), hurtInfo.Damage / 8, 6);

        if (_empressOfLight.Contains(proj.type) && hurtInfo.Damage < 1250)
            AddBuff(_empressBuff, 3 * hurtInfo.Damage, hurtInfo.Damage / 7, 1);
    }
}
