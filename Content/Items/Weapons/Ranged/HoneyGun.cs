using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Items.Weapons.Ranged;

public class HoneyGun : ModItem
{
    public override void SetDefaults()
    {
        // Copies the Slime Gun's use animation, use sound,
        // shoot speed, dimensions, and other item properties.
        Item.CloneDefaults(ItemID.SlimeGun);

        // Replace the vanilla slime projectile with ours.
        Item.shoot = ModContent.ProjectileType<HoneySprayProjectile>();

        /*
         * Give the projectile a tiny internal damage value so
         * Terraria processes NPC collision and calls OnHitNPC.
         *
         * We will reduce the actual damage to zero inside
         * the projectile.
         */
        Item.damage = 0;
        Item.knockBack = 0f;
        Item.DamageType = DamageClass.Ranged;

        Item.value = Item.buyPrice(silver: 50);
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SlimeGun)
            .AddIngredient(ItemID.BottledHoney, 50)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
