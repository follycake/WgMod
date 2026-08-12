using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.NPCs.TownNPCs.GroundedHarpy;
using WgMod.Content.NPCs.TownNPCs.Milkmaid;
using WgMod.Content.NPCs.TownNPCs.OverflowingMimic;

namespace WgMod.Common.Systems;

public class TownNPCRespawnSystem : ModSystem
{
    public static bool unlockGroundedHarpy = false;
    public static bool unlockMilkmaid = false;
    public static bool unlockOverflowingMimic = false;

    public override void ClearWorld()
    {
        unlockGroundedHarpy = false;
        unlockMilkmaid = false;
        unlockOverflowingMimic = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(unlockGroundedHarpy)] = unlockGroundedHarpy;
        tag[nameof(unlockMilkmaid)] = unlockMilkmaid;
        tag[nameof(unlockOverflowingMimic)] = unlockOverflowingMimic;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        unlockGroundedHarpy = tag.GetBool(nameof(unlockGroundedHarpy));
        unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<GroundedHarpyNPC>());
        unlockMilkmaid = tag.GetBool(nameof(unlockMilkmaid));
        unlockMilkmaid |= NPC.AnyNPCs(ModContent.NPCType<MilkmaidNPC>());
        unlockOverflowingMimic = tag.GetBool(nameof(unlockOverflowingMimic));
        unlockOverflowingMimic |= NPC.AnyNPCs(ModContent.NPCType<OverflowingMimicNPC>());
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.WriteFlags(unlockGroundedHarpy);
        writer.WriteFlags(unlockMilkmaid);
        writer.WriteFlags(unlockOverflowingMimic);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadFlags(out unlockGroundedHarpy);
        reader.ReadFlags(out unlockMilkmaid);
        reader.ReadFlags(out unlockOverflowingMimic);
    }
}
