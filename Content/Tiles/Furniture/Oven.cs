using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture;

public class Oven : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;

        AdjTiles = [TileID.CookingPots];

        AddMapEntry(new Color(200, 200, 200));

        RegisterItemDrop(ModContent.ItemType<Items.Placeable.Furniture.GraniteOven>(), 1);

        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
        TileObjectData.newTile.Origin = new Point16(1, 1);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.AnchorInvalidTiles =
        [
            TileID.MagicalIceBlock,
            TileID.Boulder,
            TileID.BouncyBoulder,
            TileID.LifeCrystalBoulder,
            TileID.RollingCactus,
        ];
        TileObjectData.newTile.AnchorBottom = new AnchorData(
            AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide,
            TileObjectData.newTile.Width,
            0
        );
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Mod.GetLocalization("Tiles.Oven.MapEntry"));
    }
}
