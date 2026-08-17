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
    public const float WeightLossRate = 5f;

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

        wp._active = true;
        wp._hidden = hideVisual;
    }
}

public class AmuletOfStarvingPlayer : ModPlayer
{
    public bool _active;
    public bool _hidden;

    public override void ResetEffects()
    {
        _active = false;
        _hidden = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (Main.rand.NextBool(30) && _active == true && _hidden == false)
        {
            Dust.NewDust(
                Player.position,
                Player.width,
                Player.height - 1,
                DustID.Shadowflame,
                0f,
                0f,
                100,
                default,
                0.7f
            );
        }
    }
}
