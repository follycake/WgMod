using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items;
using WgMod.Content.Items.Accessories.Fat;

namespace WgMod.Common.GlobalNPCs;

public class SellStraw : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        switch (shop.NpcType)
        {
            case NPCID.PartyGirl:
                shop.Add<HeliumTank>();
                shop.Add<DrinkingStraw>();
                break;
        }
    }
}