using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement;

[AutoloadEquip(EquipType.Shoes)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class TerraskeletonLegs : ModItem
{
    public const int MoveSpeedBonus = 8;
    public const int LavaImmunityTime = 2;

    public override LocalizedText Tooltip =>
        base.Tooltip.WithFormatArgs(MoveSpeedBonus, LavaImmunityTime);

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
        int prevRocketBoots = player.rocketBoots;
        player.moveSpeed += MoveSpeedBonus / 100f;
        player.accRunSpeed = 6.75f;
        player.rocketBoots = 4;
        player.vanityRocketBoots = 4;

        player.waterWalk2 = true;
        player.waterWalk = true;
        player.iceSkate = true;
        player.fireWalk = true;
        player.lavaRose = true;
        player.lavaMax += LavaImmunityTime * 60;

        if (prevRocketBoots > 0 || !player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        wg.MovementPenalty *= float.Lerp(1f, 0.6f, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.TerrasparkBoots)
            .AddIngredient<ExoskeletonLegs>()
            .AddIngredient(ItemID.HallowedBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
