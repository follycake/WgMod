using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class Infatuated : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }
    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        Player player = Main.LocalPlayer;
        int time = player.buffTime[player.FindBuffIndex(ModContent.BuffType<Infatuated>())];

        tip = string.Format(tip, (Math.Clamp(2 * (time / 60f), 5, 200) / 100f).Percent());
    }
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        player.buffTime[buffIndex] = Math.Min(player.buffTime[buffIndex] + time, Utility.TimeToTicks(minutes: 2));
        return true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.WeightGainRate += Math.Clamp(2 * (player.buffTime[buffIndex] / 60f), 5, 200) / 100f;
    }
}
