using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class ChampionsBelt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 28;

        Item.accessory = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(silver: 20);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out ChampionsBeltPlayer cb))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        cb._active = true;
        cb._meleeScale = float.Lerp(1.25f, 2f, immobility);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GoldBar, 6)
            .AddIngredient(ItemID.Ruby, 2)
            .AddIngredient(ItemID.Emerald, 2)
            .AddIngredient(ItemID.Amethyst, 2)
            .AddTile(TileID.Anvils)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.PlatinumBar, 6)
            .AddIngredient(ItemID.Ruby, 2)
            .AddIngredient(ItemID.Emerald, 2)
            .AddIngredient(ItemID.Amethyst, 2)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class ChampionsBeltPlayer : ModPlayer
{
    internal bool _active;
    internal float _meleeScale;

    public override void ResetEffects()
    {
        _active = false;
    }
}

public class ChampionsBeltScaling : GlobalItem
{
    public override void ModifyItemScale(Item item, Player player, ref float scale)
    {
        if (!player.TryGetModPlayer(out ChampionsBeltPlayer cb))
            return;
        if (cb._active && item.CountsAsClass(DamageClass.Melee))
            scale *= cb._meleeScale;
    }
}
