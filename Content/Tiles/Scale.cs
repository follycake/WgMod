using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Common.Players;

namespace WgMod.Content.Tiles;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.thegungis)]
public class Scale : ModTile
{
    public const float WeightLimit = 250f;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        DustType = DustID.Lead;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.CoordinateHeights = [18];
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(130, 130, 130), Mod.GetLocalization("Items.Scale.DisplayName"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance * 2);
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance * 2))
            return false;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return false;
        Weight weight = wg.Weight;
        if (weight.Mass > WeightLimit)
        {
            Main.NewText("Scale: ERROR - TOO HEAVY", Color.Red);
            WorldGen.KillTile(i, j, noItem: true);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.TileManipulation, number: 4, number2: i, number3: j);
        }
        else
            Main.NewText("Scale: " + weight, Color.Lime);
        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance * 2))
            return;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Scale>();
    }
}
