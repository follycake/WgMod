using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Tiles;

namespace WgMod.Common.Systems;

public class ReplaceVanillaTilesSystem : ModSystem
{
    public override void PostWorldGen()
    {
        // going through every single tile in the world... Awesome...
        // it's not the best, but it'll have to do. will also be convenient if we do more of these
        for (int x = 5; x < Main.maxTilesX - 5; x++)
        {
            for (int y = 5; y < Main.maxTilesY - 5; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Traps && tile.TileFrameY == 0 && Main.rand.NextBool(4))
                    tile.TileType = (ushort)ModContent.TileType<FatteningDartTrap>();
            }
        }
    }
}
