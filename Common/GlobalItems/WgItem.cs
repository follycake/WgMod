using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common.GlobalItems;

public class WgItem : GlobalItem
{
    public override void OnConsumeItem(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        switch (item.type)
        {
            case ItemID.LifeCrystal:
            case ItemID.LifeFruit:
                wg.AddStomach(WgPlayer.StomachCapacity);
                break;
        }
    }

    public override bool CanUseItem(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return true;
        if (WgMod._buffTable.TryGetValue(item.buffType, out GainOptions gain) && gain.IsInstant)
        {
            if (wg.Stomach + gain.TotalGain > WgPlayer.StomachCapacity)
                return false;
        }
        return true;
    }

    public override void UseAnimation(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (item.useStyle == ItemUseStyleID.Swing && wg.Weight.GetStage() >= WeightStage.MorbidlyObese)
            wg.Jiggle(3f);
    }

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        position.Y += wg._addedGfxOffY;
    }
}
