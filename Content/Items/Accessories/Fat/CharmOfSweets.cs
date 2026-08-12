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
public class CharmOfSweets : ModItem
{
    WgStat _regen = new(4f, 16f);
    WgStat _potionDelay = new(0.85f, 0.7f);

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 20;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.value = Item.buyPrice(gold: 4);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _regen.Lerp(immobility);
        _potionDelay.Lerp(immobility);

        player.lifeRegen += _regen;
        player.PotionDelayModifier *= _potionDelay;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CharmofMyths)
            .AddIngredient<PowderedSugar>(45)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient<BandOfSweetening>()
            .AddIngredient(ItemID.PhilosophersStone)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_regen / 2, ((_potionDelay - 1) * -1).Percent());
    }
}
