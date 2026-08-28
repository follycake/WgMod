using System;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Common.Systems;

public class WindSystem : ModSystem
{
    public static int windDirection;

    public static float clampedWind;

    public override void PreUpdateItems()
    {
        if (Main.windSpeedCurrent > 0)
            windDirection = 1;
        else
            windDirection = -1;

        clampedWind = Math.Clamp(Math.Abs(Main.windSpeedCurrent), 0f, 1f);
    }
}
