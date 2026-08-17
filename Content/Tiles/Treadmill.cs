using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Common.Players;
using WgMod.Content.Achievements;
using WgMod.Content.Items.Armor.YogaClothes;

namespace WgMod.Content.Tiles;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class Treadmill : ModTile
{
    public const float WeightLoss = 80f;
    public const int NextStyleHeight = 38;
    public const int InteractDistance = PlayerSittingHelper.ChairSittingMaxDistance * 2;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileID.Sets.CanBeSatOnForPlayers[Type] = true;

        DustType = DustID.Lead;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(130, 130, 130), Mod.GetLocalization("Items.Treadmill.DisplayName"));
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return settings.player.IsWithinSnappngRangeToTile(i, j, InteractDistance);
    }

    public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        int frameX = tile.TileFrameX / 18;
        int targetFrameX = 6;

        info.TargetDirection = -1;
        if (IsReversed(i, j))
        {
            info.TargetDirection = 1;
            targetFrameX = 1;
        }
        info.VisualOffset.Y -= 8f;

        info.AnchorTilePosition.X = i + (targetFrameX - frameX);
        info.AnchorTilePosition.Y = j;
        if (tile.TileFrameY % NextStyleHeight == 0)
            info.AnchorTilePosition.Y++;

        if (info.RestingEntity is Player player && player.TryGetModPlayer(out TreadmillPlayer tp))
        {
            tp._onTreadmill = true;
            tp._treadmillX = info.AnchorTilePosition.X * 16f + 8f;
        }
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (player.IsWithinSnappngRangeToTile(i, j, InteractDistance))
        {
            if (!player.TryGetModPlayer(out TreadmillPlayer tp))
                return false;
            if (tp._onTreadmill)
            {
                player.sitting.SitUp(player);
                return true;
            }
            player.GamepadEnableGrappleCooldown();
            player.sitting.SitDown(player, i, j);
            tp._treadmillX = player.Center.X;
            tp._onTreadmill = true;

            if (player.armor[0].type == ModContent.ItemType<YogaHeadband>() && player.armor[1].type == ModContent.ItemType<YogaTop>() && player.armor[2].type == ModContent.ItemType<YogaPants>())
                LetsGetPhysical.Condition.Complete();
        }
        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, InteractDistance))
            return;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Treadmill>();
        if (IsReversed(i, j))
            player.cursorItemIconReversed = true;
    }

    static bool IsReversed(int i, int j)
    {
        return Main.tile[i, j].TileFrameX / 72 < 1;
    }
}
