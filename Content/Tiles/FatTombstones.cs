using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Common.Players;

namespace WgMod.Content.Tiles;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
[Credit(ProjectRole.Idea, Contributor.maimaichubs)]
public class FatTombstones : ModTile
{
    public const int RegularCount = 3;
    public const int GoldCount = 3;

    public override void SetStaticDefaults()
    {
        Main.tileObsidianKill[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileSign[Type] = true;
        Main.tileLavaDeath[Type] = false;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.AvoidedByNPCs[Type] = true;
        TileID.Sets.TileInteractRead[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;

        DustType = DustID.Stone;
        VanillaFallbackOnModDeletion = TileID.Tombstones;
        AdjTiles = [TileID.Tombstones];

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
    }

    public override void PlaceInWorld(int i, int j, Item item)
    {
        Sign.ReadSign(i, j, true);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        Sign.KillSign(i, j);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public static int GetStyle(int type)
    {
        int style = 0;
        if (type >= ProjectileID.GraveMarker && type <= ProjectileID.Obelisk)
            style = type - 200;
        if (type >= ProjectileID.RichGravestone1 && type <= ProjectileID.RichGravestone5)
            style = type - ProjectileID.RichGravestone1 + 6;
        if (style >= 6) // Gold
            style = (style - 6) % GoldCount + RegularCount;
        else
            style %= RegularCount;
        return style;
    }
}
