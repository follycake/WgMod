using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture;

public class AltarOfCurses : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;

        AddMapEntry(new Color(81, 59, 99));

        RegisterItemDrop(ModContent.ItemType<Items.Placeable.Furniture.GraniteOven>(), 1);

        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        DustType = DustID.Corruption;

        AddMapEntry(new Color(81, 59, 99), Language.GetText("Mods.WgMod.Tiles.AltarOfCurses.MapEntry"));
    }
}
