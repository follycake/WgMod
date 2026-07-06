using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Items.Consumables.Potions.WeightGainPotions;

// TODO: Use inheritance
public class LesserWeightGainPotion : ModItem
{
	const float WeightEffect = 10;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.White;
		Item.value = Item.buyPrice(silver: 3);
	}

	public override bool? UseItem(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return false;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
		{
			weightChange = WeightEffect;
			player.AddBuff(ModContent.BuffType<MilkshakeSickness>(), 1 * 60 * 30);
		}

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
	}
}

public class WeightGainPotion : ModItem
{
	const float WeightEffect = 20;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 6);
	}

	public override bool? UseItem(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return false;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
		{
			weightChange = WeightEffect;
			player.AddBuff(ModContent.BuffType<MilkshakeSickness>(), 1 * 60 * 30);
		}

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
	}
}

public class GreaterWeightGainPotion : ModItem
{
	const float WeightEffect = 30;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(silver: 10);
	}

	public override bool? UseItem(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return false;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
		{
			weightChange = WeightEffect;
			player.AddBuff(ModContent.BuffType<MilkshakeSickness>(), 1 * 60 * 30);
		}

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
	}
}

public class SuperWeightGainPotion : ModItem
{
	const float WeightEffect = 40;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 30;
	}

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 58;
		Item.useStyle = ItemUseStyleID.DrinkLiquid;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.useTurn = true;
		Item.UseSound = SoundID.Item3;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.rare = ItemRarityID.Lime;
		Item.value = Item.buyPrice(silver: 30);
	}

	public override bool? UseItem(Player player)
	{
		if (!player.TryGetModPlayer(out WgPlayer wg))
			return false;
		float weightChange;

		if (player.HasBuff<MilkshakeSickness>())
			weightChange = WeightEffect * 0.1f;
		else
		{
			weightChange = WeightEffect;
			player.AddBuff(ModContent.BuffType<MilkshakeSickness>(), 1 * 60 * 30);
		}

		wg.SetWeight(wg.Weight + weightChange);

		CombatText.NewText(player.getRect(), Color.Yellow, weightChange + " kg");
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.FormatLines(WeightEffect);
	}
}
