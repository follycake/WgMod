using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Projectiles.Melee;

namespace WgMod.Content.Items.Weapons.Melee;

public class Sunrise : ModItem
{
    WgStat _damage = new(1f, 1.25f);
    WgStat _knockback = new(1f, 1.25f);
    WgStat _velocity = new(1f, 1.5f);

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 1, silver: 50);

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 25;
        Item.useTime = 25;
        Item.UseSound = SoundID.Item1;
        Item.autoReuse = true;

        Item.damage = 33;
        Item.knockBack = 5f;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.DamageType = DamageClass.Melee;

        Item.shootSpeed = 12f;
        Item.shoot = ModContent.ProjectileType<SunriseProjectile>();
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _damage.Lerp(immobility);
        _knockback.Lerp(immobility);
        _velocity.Lerp(immobility);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= _damage;
    }

    public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
    {
        knockback *= _knockback;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BoneJavelin, 1)
            .AddIngredient(ItemID.HellstoneBar, 24)
            .AddTile(TileID.Anvils)
            .Register();
    }
}
