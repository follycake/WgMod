using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Items.Ammo;

namespace WgMod.Content.Items.Accessories.Fat;

[AutoloadEquip(EquipType.HandsOn)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.the_trueterrafox)]
[Credit(ProjectRole.Idea, Contributor.the_trueterrafox)]
public class BandOfSweetening : ModItem
{
    WgStat _regen = new(2f, 8f);

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _regen.Lerp(immobility);

        player.lifeRegen += _regen;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BandofRegeneration)
            .AddIngredient<PowderedSugar>(45)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_regen / 2);
    }
}
