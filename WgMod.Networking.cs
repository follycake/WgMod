using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Common.Systems;
using WgMod.Content.TileEntities;

namespace WgMod;

partial class WgMod
{
    public enum MessageType : byte
    {
        Invalid = 0,
        WgPlayerSync,
        WgPlayerGurgle,
        WgPlayerCombatWeightText,
        MannequinSetStage,
        FeedingTubeSetLiquid,
        FeedingTubePlayerSync
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType type = (MessageType)reader.ReadByte();
        switch (type)
        {
            case MessageType.WgPlayerSync:
                WgPlayer player = Main.player[reader.ReadByte()].Wg();
                player.ReceivePlayerSync(reader);
                if (Main.netMode == NetmodeID.Server) // Forward the changes to the other clients
                    player.SyncPlayer(-1, whoAmI, false);
                break;
            case MessageType.WgPlayerGurgle:
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = this.GetPacket(type);
                    packet.Write(reader.ReadByte());
                    packet.Send();
                }
                else
                    Main.player[reader.ReadByte()].Wg().Gurgle(false);
                break;
            case MessageType.WgPlayerCombatWeightText:
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = this.GetPacket(type);
                    packet.Write(reader.ReadByte());
                    packet.Write(reader.ReadSingle());
                    packet.Send();
                }
                else
                    Main.player[reader.ReadByte()].Wg().CombatWeightText(reader.ReadSingle(), false);
                break;
            case MessageType.MannequinSetStage:
                int id = reader.ReadInt32();
                byte stage = reader.ReadByte();
                WgMannequinSystem.SetStage((TEDisplayDoll)TileEntity.ByID[id], stage, false);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = this.GetPacket(type);
                    packet.Write(id);
                    packet.Write(stage);
                    packet.Send();
                }
                break;
            case MessageType.FeedingTubeSetLiquid:
                id = reader.ReadInt32();
                sbyte liquidType = reader.ReadSByte();
                byte liquidAmount = reader.ReadByte();
                ((TEFeedingTube)TileEntity.ByID[id]).SetLiquid(liquidType, liquidAmount, Main.netMode == NetmodeID.Server);
                break;
            case MessageType.FeedingTubePlayerSync:
                FeedingTubePlayer fp = Main.player[reader.ReadByte()].GetModPlayer<FeedingTubePlayer>();
                fp.ReceivePlayerSync(reader);
                if (Main.netMode == NetmodeID.Server)
                    fp.SyncPlayer(-1, whoAmI, false);
                break;
            default:
                Logger.WarnFormat("WgMod: Unknown Message type: {0}", type);
                break;
        }
    }
}
