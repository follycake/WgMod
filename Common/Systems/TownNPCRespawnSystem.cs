using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.NPCs;
using WgMod.Content.NPCs.GroundedHarpy;
using WgMod.Content.NPCs.Sanguist;

namespace WgMod.Common.Systems;

public class TownNPCRespawnSystem : ModSystem
{
    public static bool unlockGroundedHarpy = false;
    public static bool unlockSanguist = false;

    public override void ClearWorld()
    {
        unlockGroundedHarpy = false;
        unlockSanguist = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag[nameof(unlockGroundedHarpy)] = unlockGroundedHarpy;
        tag[nameof(unlockSanguist)] = unlockSanguist;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        unlockGroundedHarpy = tag.GetBool(nameof(unlockGroundedHarpy));

        unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<GroundedHarpyNPC>());
        unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<SanguistNPC>());
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.WriteFlags(unlockGroundedHarpy);
        writer.WriteFlags(unlockSanguist);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadFlags(out unlockGroundedHarpy);
        reader.ReadFlags(out unlockSanguist);
    }
}
