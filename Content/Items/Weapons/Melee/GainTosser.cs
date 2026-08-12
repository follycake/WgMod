using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.alphas0)]
[Credit(ProjectRole.Artist, Contributor.alphas0)]
public class GainTosser : ModItem
{
    WgStat _damage = new(1f, 2f);
    WgStat _velocity = new(1f, 1.5f);

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.AleThrowingGlove);

        Item.width = 30;
        Item.height = 28;

        Item.damage = 25;
        Item.shootSpeed = 13f;
        Item.useTime = 26;
        Item.useAnimation = 26;

        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.buyPrice(gold: 5);

        Item.useAmmo = ModContent.ItemType<LesserWeightGainPotion>();
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _velocity.Lerp(immobility);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= _damage;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= _velocity;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines((_damage - 1f).Percent(), (_velocity - 1f).Percent());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AleThrowingGlove)
            .AddIngredient<WeightGainPotion>(10)
            .AddTile(TileID.Anvils)
            .Register();
    }

}
