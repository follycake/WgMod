using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Tools.Hammers;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class HoneyHammer : ModItem
{
	WgStat _damage = new(1f, 1.05f);
	WgStat _hammer = new(50f, 60f);
	WgStat _speed = new(1f, 1.1f);

	public override void SetDefaults()
	{
		Item.damage = 23;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 19;
		Item.useAnimation = 45;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 6;
		Item.value = Item.sellPrice(silver: 30);
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.hammer = 65;
	}

	public override void UpdateInventory(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float immobility = wg.Weight.ClampedImmobility;

		_damage.Lerp(immobility);
		_hammer.Lerp(immobility);
		_speed.Lerp(immobility);

		_hammer.Value = MathF.Floor(_hammer.Value / 5f) * 5f;

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
			.AddIngredient(ItemID.HoneyComb, 16)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
