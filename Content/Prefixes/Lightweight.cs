using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Prefixes;

public class Lightweight : ModPrefix
{
    WgStat _movePenalty = new(0.99f, 0.96f);

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

        _movePenalty.Lerp(immobility);

        wg.MovementPenalty *= _movePenalty;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "Lightweight", MovePenaltyTooltip.Format((_movePenalty.Value - 1f).Percent()))
        {
            OverrideColor = Terraria.ID.Colors.RarityPink,
        };
    }

    public static LocalizedText MovePenaltyTooltip { get; private set; }

    public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

    public override void SetStaticDefaults()
    {
        MovePenaltyTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(MovePenaltyTooltip)}");
        _ = AdditionalTooltip;
    }
}
