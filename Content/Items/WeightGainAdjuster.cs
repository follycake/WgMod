using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class WeightGainAdjuster : ModItem
{
    public override string Texture => "WgMod/Content/Items/WeightManipulator";

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 10;
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;

        Item.maxStack = 1;
        Item.value = Item.buyPrice(gold: 1);

        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = 5;
        Item.useAnimation = 5;
        Item.autoReuse = true;
        Item.noMelee = true;

        Item.UseSound = SoundID.Item4;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool CanUseItem(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        if (player.TryGetModPlayer(out WGAPlayer wga) && player.whoAmI == Main.myPlayer)
        {
            int sign = player.altFunctionUse == 2 ? -1 : 1;
            wga._multiplier = Math.Clamp(wga._multiplier + sign, -4, 40);
            if (wga._multiplier == 0)
                Main.NewText("Weight gain rate is set to default.", 255, 255, 0);
            else if (wga._multiplier > 0)
                Main.NewText($"Weight gain rate is increased by {25f * wga._multiplier}%.", 255, 255, 0);
            else
                Main.NewText($"Weight gain rate is decreased by {25f * wga._multiplier * -1}%.", 255, 255, 0);
            return true;
        }
        return null;
    }
}

public class WGAPlayer : ModPlayer
{
    internal int _multiplier = 0;

    public override void PreUpdateBuffs()
    {
        Player.Wg().WeightGainRate += 0.25f * _multiplier;
    }
}
