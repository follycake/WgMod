using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnTable : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true;

        DustType = DustID.WoodFurniture;
        AdjTiles = [TileID.Tables];

        // Placement
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

        // Etc
        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Table"));
    }

    public override void NumDust(int x, int y, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}
