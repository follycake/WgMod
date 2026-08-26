using WgMod.Content.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using WgMod.Common.Players;
using Microsoft.Xna.Framework;
using System;
using WgMod.Content.Items.Accessories.Fat;
using System.Collections.Generic;
using WgMod.Common.Systems;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class SterlingPlatter : ModItem
{
	WgStat _damage = new(1f, 1.5f);
	WgStat _knockback = new(1f, 1.5f);
	WgStat _modifier = new(1f, 1.3f);

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

		_modifier.Lerp(WindSystem.clampedWind);

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

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		if (!player.TryGetModPlayer(out TailwindsPlayer tp))
			return;

		int particles = (int)((_modifier - 1f) * 10f);

		if (player.direction == WindSystem.windDirection)
		{
			damage *= _modifier;

			if (!tp.active)
				for (int i = 0; i < particles; i++)
					Dust.NewDustDirect(position, 0, 0, DustID.Sand, velocity.X, velocity.Y, 50);
		}
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines((_modifier - 1f).Percent());
	}
}
