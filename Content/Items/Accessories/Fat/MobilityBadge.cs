using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class MobilityBadge : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 30;

        Item.accessory = true;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.buyPrice(gold: 5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer(out WgPlayer wg))
            wg.PreventImmobility = true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }
}
