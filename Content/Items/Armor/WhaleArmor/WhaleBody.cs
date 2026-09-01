using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.WhaleArmor;

[AutoloadEquip(EquipType.Body)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class WhaleBody : ModItem
{
    WgStat _damage = new(0.06f, 0.12f);
    WgStat _defense = new(25f, 35f);
    WgStat _health = new(50f, 100f);
    WgStat _fishing = new(15f, 30f);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 14;
        Item.value = Item.sellPrice(gold: 2, silver: 40);
        Item.rare = ItemRarityID.LightRed;
        Item.defense = 25;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.GetClampedFactor(WeightStage.Regular, WeightStage.Blob);

        _damage.Lerp(immobility);
        _defense.Lerp(immobility);
        _health.Lerp(immobility);
        _fishing.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        Item.defense = _defense;
        player.GetDamage(DamageClass.Generic) += _damage;
        player.statLifeMax2 += _health;
        player.fishingSkill += _fishing;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torso;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AnglerVest)
            .AddIngredient(ItemID.AdamantiteBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.AnglerVest)
            .AddIngredient(ItemID.TitaniumBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_health, _fishing, _defense - _defense.Min, _damage.Percent());
    }
}
