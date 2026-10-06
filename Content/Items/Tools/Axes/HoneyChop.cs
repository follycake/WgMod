using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Tools.Axes;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class HoneyChop : ModItem
{
	WgStat _damage = new(1f, 1.1f);
	WgStat _axe = new(70f, 80f);
	WgStat _speed = new(1f, 1.1f);

	public override void SetDefaults()
	{
		Item.damage = 20;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 15;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 6;
		Item.value = Item.sellPrice(silver: 27);
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.axe = 70 / 5;
	}

	public override void UpdateInventory(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float immobility = wg.Weight.ClampedImmobility;

		_damage.Lerp(immobility);
		_axe.Lerp(immobility);
		_speed.Lerp(immobility);

		_axe.Value = MathF.Floor(_axe.Value / 5f) * 5f;

		Item.axe = _axe / 5;
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
			.AddIngredient(ItemID.HoneyComb, 12)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
