using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Items.Ammo;

namespace WgMod.Content.Items.Tools.Axes;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class CrystalSugarHamaxe : ModItem
{
	WgStat _damage = new(1f, 1.05f);
	WgStat _axe = new(17f, 20f);
	WgStat _hammer = new(85f, 100f);
	WgStat _speed = new(1f, 1.5f);

	public override void SetDefaults()
	{
		Item.damage = 39;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 12;
		Item.useAnimation = 35;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 6;
		Item.value = Item.sellPrice(gold: 2);
		Item.rare = ItemRarityID.LightRed;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.axe = 17;
		Item.hammer = 85;
	}

	public override void UpdateInventory(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float immobility = wg.Weight.ClampedImmobility;

		_damage.Lerp(immobility);
		_axe.Lerp(immobility);
		_hammer.Lerp(immobility);
		_speed.Lerp(immobility);

		_hammer.Value = MathF.Floor(_hammer.Value / 5f) * 5f;

		Item.axe = _axe;
		Item.hammer = _hammer;
	}

	public override float UseSpeedMultiplier(Player player)
	{
		return _speed;
	}

	public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
	{
		damage *= _damage;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.CrystalShard, 12)
			.AddIngredient<PowderedSugar>(18)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
