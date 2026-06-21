using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Weapons;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class HarpyStormbow : ModItem
{
    WgStat _damage = new(1f, 1.25f);
    WgStat _crit = new(1f, 1.25f);
    WgStat _arrows = new(1, 2);

    public override void SetDefaults()
    {
        Item.width = 62;
        Item.height = 30;
        Item.scale = 1f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 2);

        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = true;

        Item.DamageType = DamageClass.Ranged;
        Item.damage = 20;
        Item.knockBack = 5f;
        Item.noMelee = true;

        Item.shoot = ProjectileID.PurificationPowder;
        Item.shootSpeed = 10f;
        Item.useAmmo = AmmoID.Arrow;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-1f, 0f);
    }


    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _arrows.Lerp(immobility);
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

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 mousePosition = Main.MouseWorld;
        Vector2 offsetPosition;
        float offsetVelocity;

        position = new Vector2(mousePosition.X + float.Lerp(-70f, 70f, Main.rand.NextFloat()), player.position.Y - 700f);

        offsetVelocity = (mousePosition.X - player.position.X) * 0.005f;

        offsetPosition = new(offsetVelocity * -70, 0);

        position += offsetPosition;
        velocity = new Vector2(offsetVelocity + float.Lerp(-1f, 1f, Main.rand.NextFloat()), 10f);

        for (int i = 0; i < _arrows; i++)
            Projectile.NewProjectile
            (
                player.GetSource_FromThis(),
                position + new Vector2(0f, float.Lerp(-100, -200, Main.rand.NextFloat())),
                new Vector2(offsetVelocity + float.Lerp(-2f, 2f, Main.rand.NextFloat()), 10f),
                type,
                damage,
                knockback
            );
    }
}
