using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class StarlightLens : ModItem
{
    WgStat _critChance = new(2f, 10f);
    WgStat _critDamage = new(0.1f, 0.75f);
    public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(silver: 75);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out StarlightLensPlayer sl))
            return;

        sl._enabled = true;

        float lerping = player.Wg().Weight.GetClampedFactor(Weight.Base, Weight.FromStage(WeightStage.Blob));

        _critChance.Lerp(lerping);
        _critDamage.Lerp(lerping);

        player.GetCritChance(DamageClass.Generic) += (int)_critChance;
        player.ExtraStats()._critDamage += _critDamage;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_critChance, _critDamage.Percent());
    }
}
public class StarlightLensPlayer : ModPlayer
{
    internal bool _enabled;
    internal int _multiplier;
    public override void ResetEffects()
    {
        _enabled = false;
        _multiplier = Math.Min(_multiplier + 1, 60);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit && _enabled)
        {
            Mass weightGain = 1f * (_multiplier / 60f);
            weightGain = Player.Wg().AddWeight(weightGain);
            _multiplier = 10;
        }
    }
}

