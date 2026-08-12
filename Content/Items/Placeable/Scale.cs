using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.thegungis)]
public class Scale : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Scale>());
        Item.width = 30;
        Item.height = 28;
        Item.value = Item.buyPrice(gold: 1);
    }
}

public class SellScale : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.Merchant)
            shop.Add<Scale>();
    }
}
