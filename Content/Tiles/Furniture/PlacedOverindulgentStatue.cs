using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture;

public class PlacedOverindulgentStatue : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.Width = 4;
        TileObjectData.newTile.Height = 4;
        TileObjectData.newTile.Origin = new Point16(2, 3);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 2, 1);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 4;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        DustType = -1;

        AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
    }
}

public class PlacedOverindulgentButt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileObsidianKill[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, 3, 1);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        DustType = -1;

        AddMapEntry(new Color(144, 148, 144), Language.GetText("MapObject.Statue"));
    }
}
