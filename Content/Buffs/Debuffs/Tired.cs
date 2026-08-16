using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.igobee_)]
public class Tired : ModBuff
{
    public const int StartStage = WeightStage.SoftImmobile;

    WgStat _attackSpeed = new(0.9f, 0.6f);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Load()
    {
        On_PlayerEyeHelper.SetStateByPlayerInfo += SetStateByPlayerInfo;
    }

    public override void Unload()
    {
        On_PlayerEyeHelper.SetStateByPlayerInfo -= SetStateByPlayerInfo;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float factor = wg.Weight.GetClampedFactor(StartStage, WeightStage.Max);
        _attackSpeed.Lerp(factor);
        player.GetAttackSpeed(DamageClass.Generic) *= _attackSpeed;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = string.Format(tip, (1f - _attackSpeed).Percent());
    }

    static void SetStateByPlayerInfo(On_PlayerEyeHelper.orig_SetStateByPlayerInfo orig, ref PlayerEyeHelper self, Player player)
    {
        if (player.HasBuff<Tired>())
            self.SwitchToState(PlayerEyeHelper.EyeState.IsTipsy);
        else
            orig(ref self, player);
    }
}
