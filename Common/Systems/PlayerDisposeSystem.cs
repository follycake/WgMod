using System;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common.Systems;

public class PlayerDisposeSystem : ModSystem
{
    void Dispose(WgPlayer wg)
    {
        if (WgPhysics.Dispose(wg))
            Mod.Logger.Debug("PLAYER DISPOSED");
    }

    public override void PostUpdateEverything()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];
            if (!player.active && player.TryGetModPlayer(out WgPlayer wg))
                Dispose(wg);
        }
    }

    public override void OnWorldUnload()
    {
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];
            if (player.TryGetModPlayer(out WgPlayer wg))
                Main.RunOnMainThread(() => Dispose(wg));
        }
    }
}
