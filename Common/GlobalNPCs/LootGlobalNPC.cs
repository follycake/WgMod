using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Melee;

namespace WgMod.Common.GlobalNPCs;

public class LootGlobalNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        switch (npc.type)
        {
            case NPCID.Deerclops:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AmuletOfStarving>(), 2));
                break;
            case NPCID.QueenSlimeBoss:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<QueenlyGluttony>(), 2));
                break;
            case NPCID.Golem:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MobilityBadge>(), 5));
                break;
        }
    }
}
