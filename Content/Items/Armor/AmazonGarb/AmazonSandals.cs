using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.AmazonGarb;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
[AutoloadEquip(EquipType.Legs)]
public class AmazonSandals : ModItem
{
    WgStat _critChance = new(6f, 12f);

    static int _glowMask;

    public override void SetStaticDefaults()
    {
        _glowMask = GlowMaskUtility.AddGlowMask(Texture + "_Legs_Glow");
    }

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 1;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _critChance.Lerp(immobility);

        player.GetCritChance(DamageClass.Melee) += _critChance;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_critChance);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 5)
            .AddIngredient(ItemID.Silk, 10)
            .Register();
    }

    public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
    {
        glowMask = _glowMask;
        glowMaskColor = Color.White;
    }
}
