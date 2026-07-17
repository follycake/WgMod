using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.AmazonGarb;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
[AutoloadEquip(EquipType.Body)]
public class AmazonToga : ModItem
{
    WgStat _damage = new(0.06f, 0.12f);
    WgStat _critChance = new(12f, 24f);

    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 28;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 1;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _damage.Lerp(immobility);
        _critChance.Lerp(immobility);

        player.GetDamage(DamageClass.Melee) += _damage;
        player.GetCritChance(DamageClass.Melee) += _critChance;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), _critChance);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 8)
            .AddIngredient(ItemID.Silk, 16)
            .Register();
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
