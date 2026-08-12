using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
[Credit(ProjectRole.Idea, Contributor.radiantluminant)]
public class GreedyFlask : ModItem
{
    // This item is unused for now, but plans to use it are in place.

    WgStat _healBonus = new(150, 200);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 28;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = 1;
        Item.consumable = true;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 6);

        Item.healLife = 150;
        Item.potion = true;
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _healBonus.Lerp(immobility);
        _healBonus.Value = MathF.Floor(_healBonus.Value / 5f) * 5f;

        Item.healLife = _healBonus;
    }

    public override bool ConsumeItem(Player player)
    {
        return false;
    }
}
