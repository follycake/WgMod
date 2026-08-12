using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WgMod.Content.Tiles.Furniture.Barn.Light;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class BarnCandle : ModTile
{
    public Asset<Texture2D> _flameTexture;

    public override void Load()
    {
        _flameTexture = ModContent.Request<Texture2D>(Texture + "_Flame");
    }

    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileLavaDeath[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileWaterDeath[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileID.Sets.DisableSmartCursor[Type] = true;

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
        TileObjectData.newTile.CoordinateHeights = [20];
        TileObjectData.newTile.DrawYOffset = -4;
        TileObjectData.newTile.WaterDeath = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;

        AddMapEntry(new Color(215, 186, 54), Language.GetText("MapObject.Candle"));

        DustType = DustID.WoodFurniture;

        TileObjectData.addTile(Type);
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    /// <summary> Toggles the tile's block state. </summary>
    /// <param name="i"> The x position in tile coordinates. </param>
    /// <param name="j"> The y position in tile coordinates. </param>
    public static void ToggleState(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int topY = j - tile.TileFrameY / 18 % 3;
        short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);

        Main.tile[i, topY].TileFrameX += frameAdjustment;

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
        }
    }

    public override void HitWire(int i, int j)
    {
        ToggleState(i, j);
    }

    public override bool RightClick(int i, int j)
    {
        ToggleState(i, j);

        return true;
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;
        player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.Barn.Light.BarnCandle>();
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];
        if (tile.TileFrameX == 0)
        {
            r = 1f;
            g = 0.75f;
            b = 1f;
        }
    }

    public override void EmitParticles(int i, int j, Tile tileCache, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        if (!visible)
            return;

        Tile tile = Main.tile[i, j];

        short frameX = tile.TileFrameX;
        short frameY = tile.TileFrameY;

        if (frameX != 0 || !Main.rand.NextBool(40))
            return;

        if (frameY / 18 % 3 == 0)
        {
            var dust = Dust.NewDustDirect(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, DustID.Torch, 0f, 0f, 100, default, 1f);

            if (!Main.rand.NextBool(3))
            {
                dust.noGravity = true;
            }

            dust.velocity *= 0.3f;
            dust.velocity.Y -= 1.5f;
        }
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];

        if (!TileDrawing.IsVisible(tile))
        {
            return;
        }

        SpriteEffects effects = SpriteEffects.None;

        Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

        int width = 16;
        int offsetY = 0;
        int height = 16;
        short frameX = tile.TileFrameX;
        short frameY = tile.TileFrameY;

        TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

        ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);

        for (int c = 0; c < 7; c++)
        {
            float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
            float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

            spriteBatch.Draw(_flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, new Rectangle(frameX, frameY, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, effects, 0f);
        }
    }
}
