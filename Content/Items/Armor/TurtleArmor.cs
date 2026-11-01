using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class TurtleArmor : GlobalItem
{
    WgStat _helmetDamage = new(0.03f, 0.09f);
    WgStat _helmetDefense = new(16, 25);
    WgStat _helmetHealth = new(10, 20);

    WgStat _scaleMailDamageCrit = new(0.04f, 0.1f);
    WgStat _scaleMailDefense = new(20, 32);
    WgStat _scaleMailHealth = new(10, 20);

    WgStat _leggingsCrit = new(2f, 6f);
    WgStat _leggingsDefense = new(13, 20);
    WgStat _leggingsHealth = new(5, 10);

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return entity.type == ItemID.TurtleHelmet || entity.type == ItemID.TurtleScaleMail || entity.type == ItemID.TurtleLeggings;
    }

    public override void SetDefaults(Item item)
    {
        if (item.type == ItemID.TurtleHelmet)
            item.defense = 16;

        if (item.type == ItemID.TurtleScaleMail)
            item.defense = 20;

        if (item.type == ItemID.TurtleLeggings)
            item.defense = 13;
    }

    public override void UpdateEquip(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        if (item.type == ItemID.TurtleHelmet)
        {
            player.GetDamage(DamageClass.Melee) -= 0.06f;

            _helmetDamage.Lerp(immobility);
            _helmetDefense.Lerp(immobility);
            _helmetHealth.Lerp(immobility);

            player.GetDamage(DamageClass.Generic) += _helmetDamage;
            item.defense = _helmetDefense;
            player.statLifeMax2 += _helmetHealth;
        }

        if (item.type == ItemID.TurtleScaleMail)
        {
            player.GetDamage(DamageClass.Melee) -= 0.08f;
            player.GetCritChance(DamageClass.Melee) -= 8f;

            _scaleMailDamageCrit.Lerp(immobility);
            _scaleMailDefense.Lerp(immobility);
            _scaleMailHealth.Lerp(immobility);

            player.GetDamage(DamageClass.Generic) += _scaleMailDamageCrit;
            player.GetCritChance(DamageClass.Generic) += _scaleMailDamageCrit * 100;
            item.defense = _scaleMailDefense;
            player.statLifeMax2 += _scaleMailHealth;
        }

        if (item.type == ItemID.TurtleLeggings)
        {
            player.GetCritChance(DamageClass.Melee) -= 4f;

            _leggingsCrit.Lerp(immobility);
            _leggingsDefense.Lerp(immobility);
            _leggingsHealth.Lerp(immobility);

            player.GetCritChance(DamageClass.Generic) += _leggingsCrit;
            item.defense = _leggingsDefense;
            player.statLifeMax2 += _leggingsHealth;
        }
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.type == ItemID.TurtleLeggings)
        {
            tooltips.ReplaceDefense(_leggingsDefense.ToString());
            tooltips.Find(t => t.Name == "Tooltip0")
                .Text = Mod.GetLocalization("Items.TurtleLeggings.Tooltip")
                .Format(_leggingsCrit, _leggingsHealth);
        }

        if (item.type == ItemID.TurtleScaleMail)
        {
            tooltips.ReplaceDefense(_scaleMailDefense.ToString());
            tooltips.Find(t => t.Name == "Tooltip0")
                .Text = Mod.GetLocalization("Items.TurtleScaleMail.Tooltip")
                .Format(_scaleMailDamageCrit.Percent(), _scaleMailHealth);
        }

        if (item.type == ItemID.TurtleHelmet)
        {
            tooltips.ReplaceDefense(_helmetDefense.ToString());
            tooltips.Find(t => t.Name == "Tooltip0")
                .Text = Mod.GetLocalization("Items.TurtleHelmet.Tooltip")
                .Format(_helmetDamage.Percent(), _helmetHealth);
        }
    }
}
