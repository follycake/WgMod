using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnWorktable : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true;

        DustType = DustID.WoodFurniture;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Mod.GetLocalization("Items.BarnWorktable.DisplayName"));
    }

    public override void NumDust(int x, int y, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}
