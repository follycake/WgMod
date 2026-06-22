using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod;

partial class WgMod
{
    public enum MessageType : byte
    {
        Invalid = 0,
        WgPlayerSync,
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType type = (MessageType)reader.ReadByte();
        switch (type)
        {
            case MessageType.WgPlayerSync:
                WgPlayer player = Main.player[reader.ReadByte()].GetModPlayer<WgPlayer>();
                player.ReceivePlayerSync(reader);
                if (Main.netMode == NetmodeID.Server) // Forward the changes to the other clients
                    player.SyncPlayer(-1, whoAmI, false);
                break;
            default:
                Logger.WarnFormat("WgMod: Unknown Message type: {0}", type);
                break;
        }
    }
}
