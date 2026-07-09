using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnSink : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.CountsAsWaterSource[Type] = true;

        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = false;
        Main.tileFrameImportant[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(100, 100, 100), Language.GetText("MapObject.Sink"));

        DustType = DustID.WoodFurniture;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}
