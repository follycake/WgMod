using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement.Boots;

[AutoloadEquip(EquipType.Shoes)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class TwilightTracers : ModItem
{
    WgStat _movePenalty = new(1f, 0.35f);
    WgStat _moveSpeed = new(0.175f, 0.235f);
    WgStat _accRunSpeed = new(8f, 10f);
    WgStat _lavaMax = new(6f, 18f);
    WgStat _wingTime = new(12f, 60f);

    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 24);
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
        _wingTime.Lerp(immobility);

        int prevRocketBoots = player.rocketBoots;
        player.moveSpeed += _moveSpeed;
        player.accRunSpeed = _accRunSpeed;
        player.rocketBoots = 3;
        player.vanityRocketBoots = 3;

        player.waterWalk2 = true;
        player.waterWalk = true;
        player.iceSkate = true;
        player.fireWalk = true;
        player.lavaRose = true;
        player.wingTimeMax += _wingTime * 60;
        player.lavaMax += _lavaMax * 60;
        player.jumpSpeedBoost += 1.8f;

        if (prevRocketBoots > 0)
            return;

        wg.MovementPenalty *= _movePenalty;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_moveSpeed.Percent(), _lavaMax, (1 - _movePenalty).Percent(), _wingTime);
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<TwilightTracers>()
            .AddIngredient(ItemID.EmpressFlightBooster)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
