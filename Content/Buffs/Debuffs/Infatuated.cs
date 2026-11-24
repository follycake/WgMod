using Terraria;
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

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.WeightGainRate += 0.75f;
    }
}
