using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Common.Players;
using WgMod.Content.TileEntities;

namespace WgMod.Content.Tiles;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class FeedingTube : ModTile
{
    public const int InteractDistance = PlayerSittingHelper.ChairSittingMaxDistance * 3;

    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;

        TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
        TileID.Sets.PreventsTileHammeringIfOnTopOfIt[Type] = true;
        TileID.Sets.AvoidedByMeteorLanding[Type] = true;

        DustType = DustID.Glass;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
        TileObjectData.newTile.HookPostPlaceMyPlayer = ModContent.GetInstance<TEFeedingTube>().Generic_HookPostPlaceMyPlayer;
        TileObjectData.addTile(Type);
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        ModContent.GetInstance<TEFeedingTube>().Kill(i, j);
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return settings.player.IsWithinSnappngRangeToTile(i, j, InteractDistance);
    }

    public override bool RightClick(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, InteractDistance) || player.HeldItem == null)
            return false;
        if (!TileEntity.TryGet(i, j, out TEFeedingTube entity))
            return false;
        if (TEFeedingTube.BucketTable.TryGetValue(player.HeldItem.type, out TEFeedingTube.BucketInfo bucketInfo))
        {
            int consumed = entity.AddLiquid(bucketInfo.Liquid, player.HeldItem.stack);
            if (consumed <= 0)
                return false;
            SoundEngine.PlaySound(SoundID.SplashWeak, player.Center);
            if (!bucketInfo.Bottomless)
            {
                player.HeldItem.stack -= consumed;
                if (player.selectedItem == 58)
                    Main.mouseItem = player.HeldItem.Clone();
            }
            return true;
        }
        if (player.TryGetModPlayer(out FeedingTubePlayer fp))
        {
            if (entity.Feedee == fp)
                fp.Connect(null);
            else if (entity.Feedee == null)
                fp.Connect(entity);
            return true;
        }
        return false;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        if (!player.IsWithinSnappngRangeToTile(i, j, InteractDistance))
            return;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.Placeable.FeedingTube>();
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (TileEntity.TryGet(i, j, out TEFeedingTube entity) && entity.LiquidAmount > 0)
        {
            fail = true; // TODO: This doesn't work with fast pickaxes in multiplayer...
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                TEFeedingTube.FluidInfo fluidInfo = TEFeedingTube.FluidTable[entity.LiquidType];
                if (fluidInfo.Bucket >= 0)
                    Item.NewItem(new EntitySource_TileBreak(i, j), entity.Position.X * 16, entity.Position.Y * 16, 32, 32, fluidInfo.Bucket, entity.LiquidAmount);
                entity.SetLiquid(-1, 0);
            }
            if (Main.netMode != NetmodeID.Server)
                Main.LocalPlayer.InterruptItemUsageIfOverTile(Type);
            SoundEngine.PlaySound(SoundID.Drown);
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        if (!TileEntity.TryGet(i, j, out TEFeedingTube entity) || entity.LiquidType < 0)
            return true;
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX > 0 || tile.TileFrameY > 0)
            return true;
        const int padding = 6;
        Vector2 drawOffset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
        Vector2 drawPosition = new Vector2(i * 16 + padding, j * 16 + padding) - Main.screenPosition + drawOffset;
        Vector2 fullSize = new(16 * 3 - padding * 2, 16 * 4 - padding * 2);
        Vector2 drawSize = new(fullSize.X, MathF.Ceiling(fullSize.Y * entity.LiquidFactor * 0.5f) * 2f);
        drawPosition.Y += fullSize.Y - drawSize.Y;
        Color color = Color.White;
        if (entity.LiquidType == LiquidID.Shimmer)
            color = LiquidRenderer.GetShimmerGlitterColor(true, i, j);
        int waterStyle = entity.LiquidType switch
        {
            LiquidID.Lava => WaterStyleID.Lava,
            LiquidID.Honey => WaterStyleID.Honey,
            _ => WaterStyleID.Purity
        };
        Texture2D texture = TextureAssets.Liquid[waterStyle].Value;
        spriteBatch.Draw(texture, drawPosition, new Rectangle(0, 8, 1, 1), color, 0f, Vector2.Zero, drawSize, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, drawPosition, new Rectangle(0, 0, 1, 4), color, 0f, Vector2.Zero, new Vector2(drawSize.X, 1f), SpriteEffects.None, 0f);
        return true;
    }
}
