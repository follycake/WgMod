using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn.Light;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class BarnChandelier : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileID.Sets.MultiTileSway[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
        TileObjectData.newTile.Height = 3;
        TileObjectData.newTile.Width = 3;
        TileObjectData.newTile.Origin = new Point16(1, 0);
        TileObjectData.newTile.DrawYOffset = -2;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
        TileObjectData.newTile.WaterDeath = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;

        AddMapEntry(new Color(215, 186, 54), Language.GetText("MapObject.Chandelier"));

        DustType = DustID.WoodFurniture;

        TileObjectData.addTile(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    /// <summary> Toggles the tile's block state. </summary>
    /// <param name="i"> The x position in tile coordinates. </param>
    /// <param name="j"> The y position in tile coordinates. </param>
    public static void ToggleState(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int topX = i - tile.TileFrameX % 54 / 18;
        int topY = j - tile.TileFrameY % 54 / 18;

        short frameAdjustment = (short)(tile.TileFrameY >= 54 ? -54 : 54);

        for (int x = topX; x < topX + 3; x++)
            for (int y = topY; y < topY + 3; y++)
            {
                Main.tile[x, y].TileFrameY += frameAdjustment;
                Wiring.SkipWire(x, y);
            }

        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendTileSquare(-1, topX, topY, 3, 3);
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        offsetY += 2;
        return;
    }

    public override void HitWire(int i, int j)
    {
        ToggleState(i, j);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameY / 54 == 0)
        {
            r = 1f;
            g = 0.75f;
            b = 1f;
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        if (TileObjectData.IsTopLeft(tile))
            Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.MultiTileVine);

        return false;
    }

    public override void AdjustMultiTileVineParameters(int i, int j, ref float? overrideWindCycle, ref float windPushPowerX, ref float windPushPowerY, ref bool dontRotateTopTiles, ref float totalWindMultiplier, ref Texture2D glowTexture, ref Color glowColor)
    {
        overrideWindCycle = 1f;
        windPushPowerY = 0;
    }
}
