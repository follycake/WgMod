using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.CrimatriarchArmor;

[AutoloadEquip(EquipType.Body)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class CrimatriarchGown : ModItem
{
    public const int MinionCount = 1;

    WgStat _damage = new(0.05f, 0.1f);

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 80);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 6;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);

        player.buffImmune[BuffID.Bleeding] = true;
        player.GetDamage(DamageClass.Summon) += _damage;
        player.maxMinions += MinionCount;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torso;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 30)
            .AddIngredient(ItemID.Bone, 25)
            .AddIngredient(ItemID.TissueSample, 15)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent());
    }
}
