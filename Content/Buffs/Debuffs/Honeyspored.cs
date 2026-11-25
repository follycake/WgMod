using System;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class Honeyspored : ModBuff
{
    public const int Ticks = 10;
    int _cooldown;

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
    }
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, Utility.TimeToTicks(minutes: 5));
        return true;
    }
    public override void Update(Player player, ref int buffIndex)
    {

        if (_cooldown < Ticks)
            _cooldown++;
        else
        {
            _cooldown = 0;
            player.Wg().AddWeight(0.1f + player.buffTime[buffIndex] / 4000f * 0.05f);
        }
    }
}
