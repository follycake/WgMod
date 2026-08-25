using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn.Light;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.subparnitragen)]
public class BarnCandelabra : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(215, 186, 54), Language.GetText("MapObject.Candelabra"));

        DustType = DustID.WoodFurniture;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
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
}
