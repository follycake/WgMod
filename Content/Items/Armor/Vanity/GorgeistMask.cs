using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Armor.Vanity;

[AutoloadEquip(EquipType.Head)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.rosiamn)]
public class GorgeistMask : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 22;

        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
        Item.maxStack = 1;
    }
}
