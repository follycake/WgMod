using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.VacuumArmor;

[AutoloadEquip(EquipType.Body)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class VacuumCrop : ModItem
{
    WgStat _attack = new(0.03f, 0.12f);
    WgStat _health = new(20, 200);
    WgStat _defense = new(0, 24 * 2);
    WgStat _resist = new(0f, 0.02f);
    WgStat _movePenalty = new(1.2f, 1.05f);

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 2);
        Item.rare = ItemRarityID.Red;
        Item.defense = 46 / 2 + 1;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _attack.Lerp(immobility);

        _health.Lerp(immobility);
        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        _defense.Lerp(immobility);
        _resist.Lerp(immobility);
        _movePenalty.Lerp(immobility);

        player.GetDamage(DamageClass.Generic) += _attack;
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
            .AddIngredient(ItemID.FragmentSolar, 5)
            .AddIngredient(ItemID.FragmentNebula, 5)
            .AddIngredient(ItemID.FragmentVortex, 5)
            .AddIngredient(ItemID.FragmentStardust, 5)
            .AddIngredient(ItemID.LunarBar, 16)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_attack.Percent(), _health, _defense, _resist.Percent(), (_movePenalty.Value - 1f).Percent());
    }

    public override void ArmorArmGlowMask(Player drawPlayer, float shadow, ref int glowMask, ref Color color)
    {
        color = Color.White;
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        glowMaskColor = Color.White;
    }
}
