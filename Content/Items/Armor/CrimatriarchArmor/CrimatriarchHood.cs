using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor;

[AutoloadEquip(EquipType.Head)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class CrimatriarchHood : ModItem
{
    public const int SetBonusMinions = 2;

    WgStat _damage = new(0.03f, 0.09f);
    WgStat _attackSpeed = new(0.98f, 0.94f);

    WgStat _setBonusDamage = new(0.05f, 0.10f);
    WgStat _setBonusAttackSpeed = new(0.95f, 0.9f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 4;
    }

    public override void SetStaticDefaults()
    {
        SetBonusText = this.GetLocalization("SetBonus");
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);

        player.GetDamage(DamageClass.Summon) += _damage;
        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= _attackSpeed;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<CrimatriarchGown>()
            && legs.type == ModContent.ItemType<CrimatriarchLeggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _setBonusDamage.Lerp(immobility);
        _setBonusAttackSpeed.Lerp(immobility);

        player.GetDamage(DamageClass.Summon) += _setBonusDamage;
        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= _setBonusAttackSpeed;
        player.maxMinions += SetBonusMinions;
        player.setBonus = SetBonusText.Format(SetBonusMinions, _setBonusDamage.Percent(), (1f - _setBonusAttackSpeed).Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Headgear;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 20)
            .AddIngredient(ItemID.Bone, 15)
            .AddIngredient(ItemID.TissueSample, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (1f - _attackSpeed).Percent());
    }
}
