using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.igobee_)]
public class Tired : ModBuff
{
    public const float AttackSpeedDecrease = 0.2f;

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
        player.GetAttackSpeed(DamageClass.Generic) *= 1f - AttackSpeedDecrease;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = string.Format(tip, AttackSpeedDecrease.Percent());
    }

    static void SetStateByPlayerInfo(On_PlayerEyeHelper.orig_SetStateByPlayerInfo orig, ref PlayerEyeHelper self, Player player)
    {
        if (player.HasBuff<Tired>())
            self.SwitchToState(PlayerEyeHelper.EyeState.IsTipsy);
        else
            orig(ref self, player);
    }
}
