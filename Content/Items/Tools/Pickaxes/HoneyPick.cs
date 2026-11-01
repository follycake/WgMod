using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Tools.Pickaxes;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class HoneyPick : ModItem
{
	WgStat _damage = new(1f, 1.05f);
	WgStat _pick = new(60f, 70f);
	WgStat _speed = new(1f, 1.1f);

	public override void SetDefaults()
	{
		Item.damage = 8;
		Item.DamageType = DamageClass.Melee;
		Item.width = 40;
		Item.height = 40;
		Item.useTime = 15;
		Item.useAnimation = 25;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.knockBack = 3;
		Item.value = Item.sellPrice(silver: 36);
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;

		Item.pick = 65;
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
			.AddIngredient(ItemID.HoneyComb, 18)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
