using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;
using WgMod.Content.Items;

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
        if (!WgServerConfig.Instance.DisableFatBuffs && wg.Weight.GetStage() >= WeightStage.Blob)
        {
            bool allow = item.useStyle == ItemUseStyleID.None; // Unrelated
            allow |= item.type == ModContent.ItemType<WeightManipulator>(); // Is dev object
            allow |= item.type == ModContent.ItemType<WeightGainAdjuster>(); // Is (also) dev object
            allow |= item.shoot != ProjectileID.None && Main.projHook[item.shoot]; // Is grappling hook
            allow |= item.mountType != -1; // Is mount
            allow |= item.useStyle == ItemUseStyleID.DrinkLiquid || item.useStyle == ItemUseStyleID.DrinkLong || item.useStyle == ItemUseStyleID.EatFood; // Is consumable
            if (!allow)
                wg._armSwing = true;
            return allow;
        }
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
