using Microsoft.Xna.Framework;
using Terraria;

namespace WgMod;

public static class PhysicsUtility
{
    /// <summary> Returns true when the player is grounded. </summary>
    public static bool CheckForSolidGround(this Entity entity)
    {
        Point min = (entity.Hitbox.BottomLeft() - new Vector2(-2, -2)).ToTileCoordinates();
        Point max = (entity.Hitbox.BottomRight() + new Vector2(2, 6)).ToTileCoordinates();
        for (int y = min.Y; y <= max.Y; y++)
        {
            for (int x = min.X; x <= max.X; x++)
            {
                if (!WorldGen.InWorld(x, y))
                    continue;
                Tile tile = Main.tile[x, y];
                if (tile != null && tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]))
                    return true;
            }
        }
        return false;
    }
}
