using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.GlobalItems;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement.Boots;

[AutoloadEquip(EquipType.Shoes)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class TerraskeletonLegs : ModItem
{
    public const float MoveSpeedBonus = 0.08f;
    public const int LavaImmunityTime = 2;

    WgStat _movePenalty = new(1f, 0.65f);

    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.buyPrice(gold: 17);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || ItemDisabling.BootsLine.Active)
            return;
        float immobility = wg.Weight.ClampedImmobility;

        int prevRocketBoots = player.rocketBoots;
        player.moveSpeed += MoveSpeedBonus;
        player.accRunSpeed = 6.75f;
        player.rocketBoots = 4;
        player.vanityRocketBoots = 4;

        player.waterWalk2 = true;
        player.waterWalk = true;
        player.iceSkate = true;
        player.fireWalk = true;
        player.lavaRose = true;
        player.lavaMax += LavaImmunityTime * 60;

        if (prevRocketBoots > 0)
            return;

        _movePenalty.Lerp(immobility);

        wg.MovementPenalty *= _movePenalty;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(MoveSpeedBonus, LavaImmunityTime, (1 - _movePenalty).Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TerrasparkBoots)
            .AddIngredient<ExoskeletonLegs>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
