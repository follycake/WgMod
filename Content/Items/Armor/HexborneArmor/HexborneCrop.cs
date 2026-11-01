using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.HexborneArmor;

[AutoloadEquip(EquipType.Body)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class HexborneCrop : ModItem
{
    WgStat _damage = new(0.03f, 0.09f);
    WgStat _manaCost = new(1.02f, 1.08f);

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 60);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 7;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _manaCost.Lerp(immobility);

        player.buffImmune[BuffID.Poisoned] = true;
        player.GetDamage(DamageClass.Magic) += _damage;
        player.manaCost *= _manaCost;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torso;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.DemoniteBar, 30)
            .AddIngredient(ItemID.PurificationPowder, 20)
            .AddIngredient(ItemID.ShadowScale, 15)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), (1 - _manaCost).Percent() * -1);
    }
}
