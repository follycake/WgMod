using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Prefixes;

public class Padded : ModPrefix
{
    WgStat _health = new(5f, 40f);

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

        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.statLifeMax2 += _health;
    }

    public override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        yield return new TooltipLine(Mod, "Padded", MaxLifeTooltip.Format(_health))
        {
            OverrideColor = Terraria.ID.Colors.RarityPink,
        };
    }

    public static LocalizedText MaxLifeTooltip { get; private set; }

    public LocalizedText AdditionalTooltip => this.GetLocalization(nameof(AdditionalTooltip));

    public override void SetStaticDefaults()
    {
        MaxLifeTooltip = Mod.GetLocalization($"{LocalizationCategory}.{nameof(MaxLifeTooltip)}");
        _ = AdditionalTooltip;
    }
}
