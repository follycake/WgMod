using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.GluttonyArmor;

[AutoloadEquip(EquipType.Head)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class GluttonyHood : ModItem
{
    WgStat _attackSpeed = new(0f, 0.02f);
    WgStat _critChance = new(0f, 3f);
    WgStat _health = new(10f, 40f);
    WgStat _resist = new(0f, 0.02f);

    public const int SetBonusSummoner = 1;
    WgStat _setBonusSpeed = new(1f, 0.9f);
    WgStat _setBonusMelee = new(1f, 1.2f);
    WgStat _setBonusMage = new(1f, 0.8f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 8;
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

        _attackSpeed.Lerp(immobility);
        _critChance.Lerp(immobility);
        _health.Lerp(immobility);
        _resist.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.GetAttackSpeed(DamageClass.Generic) -= _attackSpeed;
        player.GetCritChance(DamageClass.Generic) += _critChance;
        player.statLifeMax2 += _health;
        player.endurance += _resist;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<GluttonyCrop>()
            && legs.type == ModContent.ItemType<GluttonySkirt>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out GluttonyArmorPlayer ga))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _setBonusSpeed.Lerp(immobility);
        _setBonusMelee.Lerp(immobility);
        _setBonusMage.Lerp(immobility);

        ga._active = true;
        ga._meleeScale = _setBonusMelee;

        wg.MovementPenalty *= _setBonusSpeed;
        player.ammoCost80 = true;
        player.manaCost *= _setBonusMage;
        player.maxTurrets += SetBonusSummoner;

        player.setBonus = SetBonusText.Format(SetBonusSummoner, (1 - _setBonusMage).Percent(), (_setBonusMelee - 1).Percent(), (1 - _setBonusSpeed).Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Headgear;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 6)
            .AddIngredient(ItemID.MeteoriteBar, 6)
            .AddIngredient(ItemID.BeeWax, 6)
            .AddIngredient(ItemID.Bone, 6)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_attackSpeed.Percent(), _critChance, _health, _resist.Percent(), _attackSpeed.Percent());
    }

    public class GluttonyArmorPlayer : ModPlayer
    {
        internal bool _active;
        internal float _meleeScale;

        public override void ResetEffects()
        {
            _active = false;
        }
    }

    public class GluttonyArmorScaling : GlobalItem
    {
        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            if (!player.TryGetModPlayer(out GluttonyArmorPlayer ga) || !ga._active || !item.CountsAsClass(DamageClass.Melee))
                return;

            scale *= ga._meleeScale;
        }
    }
}
