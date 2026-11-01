using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class AmuletOfStarving : ModItem
{
    const float WeightLossRate = 10f;

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1, silver: 50);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out AmuletOfStarvingPlayer wp))
            return;
        wg.MovementWeightLossRate += WeightLossRate;

        wp.Active = true;
        wp.Hidden = hideVisual;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }
}

public class AmuletOfStarvingPlayer : ModPlayer
{
    public bool Active;
    public bool Hidden;

    public override void ResetEffects()
    {
        Active = false;
        Hidden = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!Main.rand.NextBool(30) || !Active == true || !Hidden == false)
            return;

        Dust.NewDust(Player.position, Player.width, Player.height - 1, DustID.Shadowflame, 0f, 0f, 100, default, 0.7f);
    }
}
