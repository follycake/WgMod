using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Content.Items.Placeable.Furniture.Barn;

namespace WgMod.Content.Tiles.Furniture.Barn;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnDoorOpen : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileNoSunLight[Type] = true;
        TileID.Sets.HousingWalls[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.CloseDoorID[Type] = ModContent.TileType<BarnDoorClosed>();

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

        DustType = DustID.WoodFurniture;
        AdjTiles = [TileID.OpenDoor];
        RegisterItemDrop(ModContent.ItemType<BarnDoor>(), 0);
        TileID.Sets.CloseDoorID[Type] = ModContent.TileType<BarnDoorClosed>();

        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.OpenDoor, 0));
        TileObjectData.addTile(Type);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = 1;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<BarnDoor>();
    }
}
