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
        WgPlayerPlaySound,
        WgPlayerCombatWeightText,
        MannequinSetStage,
        FeedingTubeSetLiquid,
        FeedingTubePlayerSync
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType type = (MessageType)reader.ReadByte();
        int id;
        switch (type)
        {
            case MessageType.WgPlayerSync:
                WgPlayer wg = Main.player[reader.ReadByte()].Wg();
                wg.ReceivePlayerSync(reader);
                if (Main.netMode == NetmodeID.Server) // Forward the changes to the other clients
                    wg.SyncPlayer(-1, whoAmI, false);
                break;
            case MessageType.WgPlayerPlaySound:
                id = reader.ReadByte();
                wg = Main.player[id].Wg();
                byte soundId = reader.ReadByte();
                wg.PlaySound(WgSounds.AllSounds[soundId], false);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = this.GetPacket(type);
                    packet.Write((byte)wg.Player.whoAmI);
                    packet.Write(soundId);
                    packet.Send(ignoreClient: id);
                }
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
                id = reader.ReadInt32();
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
