using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Movement;

[AutoloadEquip(EquipType.Shoes)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class ExoskeletonLegs : ModItem
{
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
        int prevRocketBoots = player.rocketBoots;
        player.accRunSpeed = 6.75f;
        player.rocketBoots = 2;
        player.vanityRocketBoots = 2;

        if (prevRocketBoots > 0 || !player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        wg.MovementPenalty *= float.Lerp(1f, 0.8f, immobility);
    }

    public override void AddRecipes()
    {
        if (ModLoader.TryGetMod("CalamityFables", out Mod calamityFables))
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(calamityFables.Find<ModItem>("WulfrumMetalScrap").Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
        else if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(calamity.Find<ModItem>("WulfrumMetalScrap").Type, 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
        else
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(ItemID.GoldBar, 12)
                .AddIngredient(ItemID.Wire, 6)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SpectreBoots)
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddIngredient(ItemID.Wire, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
