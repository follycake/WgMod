using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.WhaleArmor;

[AutoloadEquip(EquipType.Legs)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class WhaleLegs : ModItem
{
    WgStat _crit = new(0.02f, 0.06f);
    WgStat _health = new(50f, 100f);
    WgStat _fishing = new(5f, 10f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 1, silver: 80);
        Item.rare = ItemRarityID.LightRed;
        Item.defense = 15;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.GetClampedFactor(WeightStage.Regular, WeightStage.Blob);

        _crit.Lerp(immobility);
        _health.Lerp(immobility);
        _fishing.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.GetCritChance(DamageClass.Generic) += _crit;
        player.statLifeMax2 += _health;
        player.fishingSkill += _fishing;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Pants;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AnglerPants)
            .AddIngredient(ItemID.AdamantiteBar, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.AnglerPants)
            .AddIngredient(ItemID.TitaniumBar, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_health, _fishing, _crit.Percent());
    }
}
