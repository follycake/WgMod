using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Paintings;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class CozyAfternoons : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;

        DustType = DustID.WoodFurniture;
        AdjTiles = [TileID.Painting6X4];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Origin = new Point16(2, 2);
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.newTile.Width = 6;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
        TileObjectData.newTile.AnchorWall = true;
        TileObjectData.addTile(Type);

        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;

        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Painting"));
    }

    public override void NumDust(int x, int y, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}
