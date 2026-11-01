using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Content.NPCs.TownNPCs.GroundedHarpy;
using WgMod.Content.NPCs.TownNPCs.Milkmaid;
using WgMod.Content.NPCs.TownNPCs.OverflowingMimic;
using WgMod.Content.NPCs.GroundedHarpy;
using WgMod.Content.NPCs.Sanguist;

namespace WgMod.Common.Systems;

public class TownNPCRespawnSystem : ModSystem
{
        public static bool unlockGroundedHarpy;
        public static bool unlockMilkmaid;
        public static bool unlockOverflowingMimic;
        public static bool unlockSanguist = false;

        public override void ClearWorld()
        {
                unlockGroundedHarpy = false;
                unlockMilkmaid = false;
                unlockOverflowingMimic = false;
                unlockSanguist = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
                tag[nameof(unlockGroundedHarpy)] = unlockGroundedHarpy;
                tag[nameof(unlockMilkmaid)] = unlockMilkmaid;
                tag[nameof(unlockOverflowingMimic)] = unlockOverflowingMimic;
                tag[nameof(unlockSanguist)] = unlockSanguist;
        }

        public override void LoadWorldData(TagCompound tag)
        {
                unlockGroundedHarpy = tag.GetBool(nameof(unlockGroundedHarpy));
                unlockSanguist = tag.GetBool(nameof(unlockSanguist));
                unlockGroundedHarpy |= NPC.AnyNPCs(ModContent.NPCType<GroundedHarpyNPC>());
                unlockMilkmaid = tag.GetBool(nameof(unlockMilkmaid));
                unlockMilkmaid |= NPC.AnyNPCs(ModContent.NPCType<MilkmaidNPC>());
                unlockOverflowingMimic = tag.GetBool(nameof(unlockOverflowingMimic));
                unlockOverflowingMimic |= NPC.AnyNPCs(ModContent.NPCType<OverflowingMimicNPCNPC>());
                unlockSanguist |= NPC.AnyNPCs(ModContent.NPCType<SanguistNPC>());
        }

        public override void NetSend(BinaryWriter writer)
        {
                writer.WriteFlags(unlockGroundedHarpy, unlockMilkmaid, unlockOverflowingMimic);
                writer.WriteFlags(unlockSanguist);
        }

        public override void NetReceive(BinaryReader reader)
        {
                reader.ReadFlags(out unlockGroundedHarpy, out unlockMilkmaid, out unlockOverflowingMimic);
                reader.ReadFlags(out unlockSanguist);
        }
}
