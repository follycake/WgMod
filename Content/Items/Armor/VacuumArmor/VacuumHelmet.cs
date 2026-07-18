using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor;

[AutoloadEquip(EquipType.Head)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class VacuumHelmet : ModItem
{
    public const float SetBonusWeightLoss = 0.1f;

    WgStat _critChance = new(2f, 8f);
    WgStat _health = new(10, 100);
    WgStat _defense = new(0, 12 * 2);
    WgStat _resist = new(0f, 0.02f);
    WgStat _movePenalty = new(1.2f, 1.05f);
    WgStat _setBonusRegen = new(0, 20);
    WgStat _setBonusHealth = new(0f, 1f);

    static int _glowMask;

    public override void SetStaticDefaults()
    {
        _glowMask = GlowMaskUtility.AddGlowMask(Texture + "_Head_Glow");

        SetBonusText = this.GetLocalization("SetBonus");
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 2);
        Item.rare = ItemRarityID.Red;
        Item.defense = 36 / 2;
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<VacuumCrop>()
            && legs.type == ModContent.ItemType<VacuumSkirt>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _setBonusRegen.Lerp(immobility);
        _setBonusHealth.Lerp(immobility);

        player.lifeRegen += _setBonusRegen;
        player.statLifeMax2 = (int)Math.Round(player.statLifeMax2 * (1f + _setBonusHealth));
        wg.WeightLossRate *= SetBonusWeightLoss;

        player.setBonus = SetBonusText.Format(_setBonusRegen, _setBonusHealth.Percent(), (1f - SetBonusWeightLoss).Percent());
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _critChance.Lerp(immobility);

        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        _defense.Lerp(immobility);
        _resist.Lerp(immobility);
        _movePenalty.Lerp(immobility);

        player.GetCritChance(DamageClass.Generic) += _critChance;
        player.statLifeMax2 += _health;
        player.statDefense += _defense;
        player.endurance += _resist;
        wg.MovementPenalty *= _movePenalty;

        player.aggro += 5;

        Vector3 light = new(130f / 255f, 90f / 255f, 190f / 255f);

        if (!Main.dedServ)
            Lighting.AddLight(player.Center, light);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 2)
            .AddIngredient(ItemID.FragmentNebula, 2)
            .AddIngredient(ItemID.FragmentVortex, 2)
            .AddIngredient(ItemID.FragmentStardust, 2)
            .AddIngredient(ItemID.LunarBar, 8)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_critChance, _health, _defense, _resist.Percent(), (_movePenalty.Value - 1f).Percent());
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        glowMask = _glowMask;
        glowMaskColor = Color.White;
    }
}
