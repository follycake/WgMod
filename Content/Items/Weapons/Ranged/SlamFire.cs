using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Weapons.Ranged;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.evanescentsummer)]
public class SlamFire : ModItem
{
	const int SoberAmount = 40 * 60;

	int _numProjectiles = 6;

	float _randomDeviation = 3f;

	public override void SetDefaults()
	{
		Item.width = 70;
		Item.height = 20;
		Item.rare = ItemRarityID.LightRed;

		Item.useTime = 70;
		Item.useAnimation = 70;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.UseSound = WgSounds.SlamFire;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 35;
		Item.knockBack = 6f;
		Item.noMelee = true;

		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 18.13f;
		Item.useAmmo = AmmoID.Bullet;
	}

	public override float UseSpeedMultiplier(Player player)
	{
		if (player.HasBuff(BuffID.Tipsy) && player.buffTime[player.FindBuffIndex(BuffID.Tipsy)] > SoberAmount)
			return 3;

		return 1;
	}

	public static void ConsumeBuff(Player player, int type, int amount)
	{
		int index = player.FindBuffIndex(type);

		if (index == -1)
			return;

		if (player.buffTime[index] > amount)
			player.buffTime[index] -= amount;
		else
			player.DelBuff(index);
	}

	public override void UpdateInventory(Player player)
	{
		if (player.tipsy && player.buffTime[player.FindBuffIndex(BuffID.Tipsy)] > SoberAmount)
		{
			_numProjectiles = 9;
			_randomDeviation = 8f;
		}
		else
		{
			_numProjectiles = 6;
			_randomDeviation = 3f;
		}
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{

		if (player.tipsy)
			ConsumeBuff(player, BuffID.Tipsy, SoberAmount);

		for (int i = 0; i < _numProjectiles; i++)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(_randomDeviation));

			newVelocity *= 1f - Main.rand.NextFloat(0.5f);

			Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
		}

		return false;
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(-12f, 8f);
	}
}
