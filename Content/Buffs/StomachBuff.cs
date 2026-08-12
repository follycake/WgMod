using Terraria;
using Terraria.ID;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class StomachBuff : WgBuffBase
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        return wg.Stomach / WgPlayer.StomachCapacity;
    }

    public override bool RightClick(int buffIndex)
    {
        return false;
    }
}
