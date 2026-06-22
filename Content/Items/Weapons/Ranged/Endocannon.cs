using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Projectiles.Ranged;

namespace WgMod.Content.Items.Weapons.Ranged;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class Endocannon : ModItem
{
    public int _cooldown;

    WgStat _damage = new(1f, 1.25f);
    WgStat _crit = new(1f, 1.25f);

    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 40;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1);

        Item.useTime = 8;
        Item.useAnimation = 8;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = true;

        Item.UseSound = SoundID.Item11;

        Item.DamageType = DamageClass.Ranged;
        Item.damage = 14;
        Item.knockBack = 5f;
        Item.noMelee = true;

        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10f;
        Item.useAmmo = AmmoID.Bullet;
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _crit.Lerp(immobility);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= _damage;
    }

    public override void ModifyWeaponCrit(Player player, ref float crit)
    {
        crit *= _crit;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.IllegalGunParts)
            .AddIngredient(ItemID.Marble, 12)
            .AddIngredient(ItemID.GoldBar, 12)
            .AddTile(TileID.Furnaces)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.IllegalGunParts)
            .AddIngredient(ItemID.Marble, 12)
            .AddIngredient(ItemID.PlatinumBar, 12)
            .AddTile(TileID.Furnaces)
            .Register();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(2f, -2f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        if (_cooldown > (int)float.Lerp(4, 9, immobility))
        {
            type = ModContent.ProjectileType<Plate>();

            velocity *= 0.75f;
            damage *= 3;

            SoundEngine.PlaySound(SoundID.Item10, position);

            _cooldown = 0;
        }
        else
            _cooldown++;

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * 18f;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
        }
    }
}
