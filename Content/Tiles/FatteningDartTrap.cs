using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Tiles;

public class FatteningDartTrap : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.DrawsWalls[Type] = true;
        TileID.Sets.DontDrawTileSliced[Type] = true;
        TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
        TileID.Sets.IsAMechanism[Type] = true;

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileFrameImportant[Type] = true;

        AddMapEntry(new Color(255,225,25), Language.GetText("MapObject.Trap"));
    }

    public override bool IsTileDangerous(int i, int j, Player player) => true;

    public override bool CreateDust(int i, int j, ref int type)
    {
        type = DustID.t_Honey;
        return true;
    }

    public override void PlaceInWorld(int i, int j, Item item)
    {
        Tile tile = Main.tile[i, j];
        if (Main.LocalPlayer.direction == 1)
            tile.TileFrameX += 18;
        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
    }

    static readonly int[] _frameXCycle = [2, 3, 4, 5, 1, 0];
    public override bool Slope(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int nextFrameX = _frameXCycle[tile.TileFrameX / 18];
        tile.TileFrameX = (short)(nextFrameX * 18);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        return false;
    }

    public override void HitWire(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        Vector2 spawnPosition;
        int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
        int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
        if (Wiring.CheckMech(i, j, 200))
        {
            spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection);

            Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 12f, ModContent.ProjectileType<FatteningDart>(), 25, 2f, Main.myPlayer);
        }
    }
}
