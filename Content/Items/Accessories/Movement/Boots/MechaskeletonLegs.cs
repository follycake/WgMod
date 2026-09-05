using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement.Boots;

[AutoloadEquip(EquipType.Shoes)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class MechaskeletonLegs : ModItem
{
    WgStat _movePenalty = new(1f, 0.5f);
    WgStat _moveSpeed = new(0.1f, 0.16f);
    WgStat _accRunSpeed = new(7f, 9f);
    WgStat _lavaMax = new(4f, 12f);

    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.buyPrice(gold: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _movePenalty.Lerp(immobility);
        _moveSpeed.Lerp(immobility);
        _accRunSpeed.Lerp(immobility);
        _lavaMax.Lerp(immobility);

        int prevRocketBoots = player.rocketBoots;
        player.moveSpeed += _moveSpeed;
        player.accRunSpeed = _accRunSpeed;
        player.rocketBoots = 1;
        player.vanityRocketBoots = 1;

        player.waterWalk2 = true;
        player.waterWalk = true;
        player.iceSkate = true;
        player.fireWalk = true;
        player.lavaRose = true;
        player.lavaMax += _lavaMax * 60;

        if (prevRocketBoots > 0)
            return;

        wg.MovementPenalty *= _movePenalty;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_moveSpeed, _lavaMax, (1 - _movePenalty).Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<TerraskeletonLegs>()
            .AddIngredient(ItemID.AdamantiteBar, 12)
            .AddIngredient(ItemID.SoulofLight, 4)
            .AddIngredient(ItemID.SoulofNight, 4)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();

        CreateRecipe()
            .AddIngredient<TerraskeletonLegs>()
            .AddIngredient(ItemID.TitaniumBar, 12)
            .AddIngredient(ItemID.SoulofLight, 4)
            .AddIngredient(ItemID.SoulofNight, 4)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
