using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WgMod.Common.Systems;

public class EventSystem : ModSystem
{
    public const int MonthLength = 30;

    public static int dayNumber;
    public static bool harpyMigration;

    public override void PostWorldGen()
    {
        dayNumber = Main.rand.Next(0, 31);
    }

    public override void PostUpdateTime()
    {
        harpyMigration = false;

        if (Main.dayTime && Main.time == 0)
            dayNumber++;

        if (dayNumber > MonthLength)
            dayNumber = 0;

        if (dayNumber == 15)
            harpyMigration = true;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(dayNumber)] = dayNumber;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        dayNumber = tag.GetInt(nameof(dayNumber));
    }
}
