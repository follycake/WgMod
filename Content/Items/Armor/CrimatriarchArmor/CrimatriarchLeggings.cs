using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor;

[AutoloadEquip(EquipType.Legs)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class CrimatriarchLeggings : ModItem
{
    WgStat _damage = new(0.03f, 0.09f);
    WgStat _attackSpeed = new(0.98f, 0.94f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 18;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 5;
    }

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

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Pants;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 25)
            .AddIngredient(ItemID.Bone, 20)
            .AddIngredient(ItemID.TissueSample, 10)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (1f - _attackSpeed).Percent());
    }
}
