using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.GluttonyArmor;

[AutoloadEquip(EquipType.Legs)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class GluttonySkirt : ModItem
{
    WgStat _attackSpeed = new(0f, 0.02f);
    WgStat _critChance = new(0f, 3f);
    WgStat _health = new(10f, 40f);
    WgStat _resist = new(0f, 0.02f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1, silver: 20);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 8;
    }

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

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Pants;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.MeteoriteBar, 5)
            .AddIngredient(ItemID.BeeWax, 5)
            .AddIngredient(ItemID.Bone, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_attackSpeed.Percent(), _critChance, _health, _resist.Percent());
    }
}
