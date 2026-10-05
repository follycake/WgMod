using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Consumables;
using WgMod.Content.Items.Ammo;

namespace WgMod.Content.Items.Tools.Pickaxes;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class CrystalSugarPickaxe : ModItem
{
	WgStat _damage = new(1f, 1.05f);
	WgStat _pick = new(150f, 165f);
	WgStat _speed = new(1f, 1.5f);

	public override void SetDefaults()
	{
		Item.damage = 15;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 12;
		Item.useAnimation = 25;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 5;
		Item.value = Item.sellPrice(gold: 1, silver: 60);
		Item.rare = ItemRarityID.LightRed;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.pick = 150;
	}

	public override void UpdateInventory(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return;
		float immobility = wg.Weight.ClampedImmobility;

		_damage.Lerp(immobility);
		_pick.Lerp(immobility);
		_speed.Lerp(immobility);

		_pick.Value = MathF.Floor(_pick.Value / 5f) * 5f;

		Item.pick = _pick;
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
			.AddIngredient(ItemID.CrystalShard, 10)
			.AddIngredient<PowderedSugar>(15)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
