using System;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Prefixes;

public abstract class DynamicPrefix : ModPrefix
{
    public abstract void UpdateWgStats(Item item, WgPlayer wg);

    public void Update(Item item, Player player)
    {
        item.SetDefaults(item.type);
        if (player.TryGetModPlayer(out WgPlayer wg))
            UpdateWgStats(item, wg);
        float dmg = 1f;
        float kb = 1f;
        float spd = 1f;
        float size = 1f;
        float shtspd = 1f;
        float mcst = 1f;
        int crt = 0;
        SetStats(ref dmg, ref kb, ref spd, ref size, ref shtspd, ref mcst, ref crt);
        item.damage = (int)Math.Round(item.damage * dmg);
        item.useAnimation = (int)Math.Round(item.useAnimation * spd);
        item.useTime = (int)Math.Round(item.useTime * spd);
        item.reuseDelay = (int)Math.Round(item.reuseDelay * spd);
        item.mana = (int)Math.Round(item.mana * mcst);
        item.knockBack *= kb;
        item.scale *= size;
        item.shootSpeed *= shtspd;
        item.crit += crt;
        Apply(item);
    }
}

public class DynamicPrefixItem : GlobalItem
{
    public override void UpdateInventory(Item item, Player player)
    {
        if (PrefixLoader.GetPrefix(item.prefix) is DynamicPrefix prefix)
            prefix.Update(item, player);
    }
}
