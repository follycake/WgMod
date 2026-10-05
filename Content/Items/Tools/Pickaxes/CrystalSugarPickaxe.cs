using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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

	bool _growth;

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

		if (Main.dayTime && Main.time == 0)
			_growth = true;
	}

	public override float UseSpeedMultiplier(Player player)
	{
		return _speed;
	}

	public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
	{
		damage *= _damage;
	}

	public override bool ConsumeItem(Player player)
	{
		return false;
	}

	public override bool CanRightClick()
	{
		if (_growth)
			return true;
		return false;
	}

	public override void RightClick(Player player)
	{
		player.AddBuff(ModContent.BuffType<CrystalSugarFrenzy>(), 30 * 60);

		SoundEngine.PlaySound(SoundID.Item2, player.Center);

		_growth = false;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (_growth)
			tooltips.Insert(6, new(Mod, "Growth", Language.GetTextValue("Mods.WgMod.GlobalItem.Growth")));
	}

	public override void LoadData(TagCompound tag)
	{
		if (!tag.TryGet(nameof(_growth), out _growth))
			_growth = false;
	}

	public override void SaveData(TagCompound tag)
	{
		tag[nameof(_growth)] = _growth;
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
