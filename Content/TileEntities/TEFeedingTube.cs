using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Players;

namespace WgMod.Content.TileEntities;

public class TEFeedingTube : ModTileEntity
{
    public const int Capacity = 100;

    public record struct BucketInfo(int Liquid, bool Bottomless = false);
    public record struct FluidInfo(int Bucket, Mass Gain);

    public static readonly Dictionary<int, BucketInfo> BucketTable = new()
    {
        [ItemID.WaterBucket] = new(LiquidID.Water),
        [ItemID.LavaBucket] = new(LiquidID.Lava),
        [ItemID.HoneyBucket] = new(LiquidID.Honey),

        [ItemID.BottomlessBucket] = new(LiquidID.Water, true),
        [ItemID.BottomlessLavaBucket] = new(LiquidID.Lava, true),
        [ItemID.BottomlessHoneyBucket] = new(LiquidID.Honey, true),
        [ItemID.BottomlessShimmerBucket] = new(LiquidID.Shimmer, true)
    };

    public static readonly Dictionary<int, FluidInfo> FluidTable = new()
    {
        [-1] = new(-1, 0f),
        [LiquidID.Water] = new(ItemID.WaterBucket, 0.1f),
        [LiquidID.Lava] = new(ItemID.LavaBucket, 0.5f),
        [LiquidID.Honey] = new(ItemID.HoneyBucket, 2f),
        [LiquidID.Shimmer] = new(-1, 4f)
    };

    public bool IsEmpty => LiquidType < 0;
    public int LiquidType { get; private set; } = -1;
    public int LiquidAmount { get; private set; }
    public float LiquidFactor => LiquidAmount / (Capacity - 1f);
    public FeedingTubePlayer Feedee { get; private set; }

    public void SetFeedeeServer(FeedingTubePlayer feedee)
    {
        if (Feedee != feedee)
        {
            Feedee = feedee;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID);
        }
    }

    public void SetLiquid(int type, int amount, bool network = true)
    {
        if (amount <= 0)
            type = -1;
        LiquidType = type;
        LiquidAmount = Math.Clamp(amount, 0, Capacity);
        if (!network || Main.netMode == NetmodeID.SinglePlayer)
            return;
        if (Main.netMode == NetmodeID.Server)
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID);
        else
        {
            ModPacket packet = Mod.GetPacket(WgMod.MessageType.FeedingTubeSetLiquid);
            packet.Write(ID);
            packet.Write((sbyte)LiquidType);
            packet.Write((byte)LiquidAmount);
            packet.Send();
        }
    }

    public int AddLiquid(int type, int amount, bool network = true)
    {
        if (LiquidType >= 0 && LiquidType != type)
            return 0;
        int startAmount = LiquidAmount;
        SetLiquid(type, startAmount + amount, network);
        return LiquidAmount - startAmount;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(LiquidType)] = LiquidType;
        tag[nameof(LiquidAmount)] = LiquidAmount;
    }

    public override void LoadData(TagCompound tag)
    {
        if (!tag.TryGet(nameof(LiquidType), out int type))
            type = -1;
        if (!tag.TryGet(nameof(LiquidAmount), out int amount))
            amount = 0;
        SetLiquid(type, amount, false);
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write((sbyte)LiquidType);
        writer.Write((byte)LiquidAmount);
        writer.Write(Feedee != null);
        if (Feedee != null)
            writer.Write((byte)Feedee.Player.whoAmI);
    }

    public override void NetReceive(BinaryReader reader)
    {
        SetLiquid(reader.ReadSByte(), reader.ReadByte(), false);
        if (reader.ReadBoolean())
            Feedee = Main.player[reader.ReadByte()].GetModPlayer<FeedingTubePlayer>();
        else
            Feedee = null;
    }

    public override bool IsTileValidForEntity(int x, int y)
    {
        Tile tile = Main.tile[x, y];
        return tile.HasTile && tile.TileType == ModContent.TileType<Tiles.FeedingTube>();
    }
}
