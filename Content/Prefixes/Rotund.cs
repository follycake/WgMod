using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Prefixes;

public class Rotund : ModPrefix
{
    WgStat _damage = new(0.01f, 0.08f);
    WgStat _attackSpeed = new(0.99f, 0.96f);

    public override PrefixCategory Category => PrefixCategory.Accessory;

    public override float RollChance(Item item)
    {
        return 1f;
    }

    public override bool CanRoll(Item item)
    {
        return true;
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult *= 1.44f;
    }

    public override void ApplyAccessoryEffects(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);

        player.GetDamage(DamageClass.Generic) += _damage;
        player.GetAttackSpeed(DamageClass.Generic) *= _attackSpeed;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "Rotund", DamageAttackSpeedTooltip.Format(_damage.Percent(), (_attackSpeed - 1).Percent()))
        {
            OverrideColor = Terraria.ID.Colors.RarityPink,
        };
    }

    public static LocalizedText DamageAttackSpeedTooltip { get; private set; }

    public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

    public override void SetStaticDefaults()
    {
        DamageAttackSpeedTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(DamageAttackSpeedTooltip)}");
        _ = AdditionalTooltip;
    }
}
