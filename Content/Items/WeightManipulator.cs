using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
public class WeightManipulator : ModItem
{
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
        if (player.TryGetModPlayer(out WgPlayer wg) && wg.OwnsPlayer())
        {
            int sign = player.altFunctionUse == 2 ? -1 : 1;
            wg.SetWeight(wg.Weight + sign * 10f);
            Main.NewText($"Weight: {wg.Weight}, Stage: {wg.Weight.GetStage()}", 255, 255, 0);
            return true;
        }
        return null;
    }
}
