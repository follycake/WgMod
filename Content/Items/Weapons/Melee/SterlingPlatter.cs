using WgMod.Content.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class SterlingPlatter : ModItem
{
	WgStat _damage = new(1f, 1.5f);
	WgStat _knockback = new(1f, 1.5f);

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.LightDisc);
		Item.shoot = ModContent.ProjectileType<SterlingPlatterProjectile>();

		Item.damage = 18;
		Item.knockBack = 3f;
		Item.shootSpeed = 15f;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateInventory(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float immobility = wg.Weight.ClampedImmobility;

		_damage.Lerp(immobility);
		_knockback.Lerp(immobility);
	}

	public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
	{
		damage *= _damage;
	}

	public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
	{
		knockback *= _knockback;
	}

}
