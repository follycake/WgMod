using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class FullOfSpider : ModBuff
{
    readonly int _minions = 1;
    WgStat _summonDamage = new(0.05f, 0.15f);

    public const int TicksPerCycle = 240;
    public const int FatPerCycle = 1;
    public int _cooldown = 0;

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out FullOfSpiderPlayer fosp))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _summonDamage.Lerp(immobility);

        player.GetDamage(DamageClass.Summon) += _summonDamage;
        player.maxMinions += _minions;

        fosp._active = true;

        if (_cooldown < TicksPerCycle)
            _cooldown++;
        else
        {
            _cooldown = 0;
            wg.CombatWeightText(wg.AddWeight(FatPerCycle), false);
            SoundEngine.PlaySound(WgSounds.Gulp, player.Center);
        }
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = base.Description.Format(_summonDamage.Percent(), _minions);
    }
}

public class FullOfSpiderPlayer : ModPlayer
{
    public bool _active;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!_active || Player.whoAmI != Main.myPlayer)
            return;

        r = 214f / 255f;
        g = 53f / 255f;
        b = 217f / 255f;
    }
}
