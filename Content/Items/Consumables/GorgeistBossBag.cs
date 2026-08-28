using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Armor.Vanity;
using WgMod.Content.Items.Weapons.Melee;
using WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;

namespace WgMod.Content.Items.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class GorgeistBossBag : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Type] = true;
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 24;
        Item.height = 24;
        Item.rare = ItemRarityID.Purple;
        Item.expert = true;
    }

    public override bool CanRightClick()
    {
        return true;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GorgeistMask>(), 7));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SterlingPlatter>(), 3));
        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Tailwinds>()));
        itemLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 1, 12, 16));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Gorgeist>()));
    }
}
