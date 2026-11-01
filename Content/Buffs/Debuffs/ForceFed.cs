using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class ForceFed : ModBuff
{
    public const int TicksPerCycle = 30;
    public const int FatPerCycle = 2;
    int _cooldown;

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override bool ReApply(Player player, int time, int buffIndex)
    {
        _cooldown += TicksPerCycle / 2;
        return false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        if (_cooldown < TicksPerCycle)
            _cooldown++;
        else
        {
            _cooldown = 0;
            wg.CombatWeightText(FatPerCycle, false);
            wg.AddStomach(FatPerCycle);
            SoundEngine.PlaySound(WgSounds.Gulp, player.Center);
        }
    }
}
