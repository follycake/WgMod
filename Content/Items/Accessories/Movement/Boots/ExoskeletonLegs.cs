using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement.Boots;

[AutoloadEquip(EquipType.Shoes)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class ExoskeletonLegs : ModItem
{
    WgStat _movePenalty = new(1f, 0.8f);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        int prevRocketBoots = player.rocketBoots;
        player.accRunSpeed = 6.75f;
        player.rocketBoots = 2;
        player.vanityRocketBoots = 2;

        if (prevRocketBoots > 0)
            return;

        _movePenalty.Lerp(immobility);

        wg.MovementPenalty *= _movePenalty;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines((1 - _movePenalty).Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        if (ModLoader.TryGetMod("CalamityFables", out Mod calamityFables))
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(calamityFables.Find<ModItem>("WulfrumMetalScrap").Type, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        else if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(calamity.Find<ModItem>("WulfrumMetalScrap").Type, 12)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
        else
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(ItemID.GoldBar, 12)
                .AddIngredient(ItemID.Wire, 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddIngredient(ItemID.Wire, 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
