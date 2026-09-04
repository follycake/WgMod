using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.GluttonyArmor;

[AutoloadEquip(EquipType.Body)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class GluttonyCrop : ModItem
{
    WgStat _damage = new(0f, 0.05f);
    WgStat _attackSpeed = new(0f, 0.04f);
    WgStat _health = new(10f, 40f);
    WgStat _resist = new(0f, 0.02f);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 14;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 9;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;

        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);
        _health.Lerp(immobility);
        _resist.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.GetDamage(DamageClass.Generic) += _damage;
        player.GetAttackSpeed(DamageClass.Generic) -= _attackSpeed;
        player.statLifeMax2 += _health;
        player.endurance += _resist;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torso;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 7)
            .AddIngredient(ItemID.MeteoriteBar, 7)
            .AddIngredient(ItemID.BeeWax, 7)
            .AddIngredient(ItemID.Bone, 7)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), _attackSpeed.Percent(), _health, _resist.Percent());
    }
}
