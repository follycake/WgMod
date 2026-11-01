using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class TileBreakerHook : ILoadable
{
    public void Load(Mod mod)
    {
        On_Player.CheckCrackedBrickBreak += CheckCrackedBrickBreak;
    }

    public void Unload()
    {
        On_Player.CheckCrackedBrickBreak -= CheckCrackedBrickBreak;
    }

    void CheckCrackedBrickBreak(On_Player.orig_CheckCrackedBrickBreak orig, Player self)
    {
        if (!self.TryGetModPlayer(out WgPlayer wg))
        {
            orig(self);
            return;
        }

        if (self.shimmering)
            return;

        int boostedChance = wg.Weight.GetStage();
        boostedChance = boostedChance * boostedChance / 2;
        bool flag = false;
        if ((float)Main.rand.Next(2, 12) - boostedChance < Math.Abs(self.velocity.X))
            flag = true;

        if ((float)Main.rand.Next(2, 12) - boostedChance < self.velocity.Y)
            flag = true;

        if (flag && self.velocity.Y < 1f && boostedChance < 8)
        {
            Point point = (self.Bottom + Vector2.UnitY).ToTileCoordinates();
            Point point2 = (self.BottomLeft + Vector2.UnitY).ToTileCoordinates();
            Point point3 = (self.BottomRight + Vector2.UnitY).ToTileCoordinates();
            if (WorldGen.SolidTileAllowBottomSlope(point.X, point.Y) &&
                !TileID.Sets.CrackedBricks[Main.tile[point.X, point.Y].TileType]
                || WorldGen.SolidTileAllowBottomSlope(point2.X, point2.Y) &&
                !TileID.Sets.CrackedBricks[Main.tile[point2.X, point2.Y].TileType]
                || WorldGen.SolidTileAllowBottomSlope(point3.X, point3.Y) &&
                !TileID.Sets.CrackedBricks[Main.tile[point3.X, point3.Y].TileType])
                flag = false;
        }

        if (!flag)
            return;

        Vector2 vector = self.position + self.velocity;
        flag = false;
        int num = (int)(vector.X / 16f);
        int num2 = (int)((vector.X + self.width) / 16f);
        int num3 = (int)((self.position.Y + self.height + 1f) / 16f);
        if (boostedChance >= 8)
        {
            num--;
            num2++;
        }

        Rectangle rect = self.getRect();
        rect.Inflate(1, 1);
        if (boostedChance < 8)
        {
            for (int i = num; i <= num2; i++)
            {
                int j = num3;
                while (j <= num3 + 1 && Main.tile[i, j] != null)
                {
                    if (Main.tile[i, j].HasUnactuatedTile && !WorldGen.SolidTile(i, j - 1) &&
                        TileID.Sets.CrackedBricks[Main.tile[i, j].TileType] &&
                        new Rectangle(i * 16, j * 16, 16, 16).Intersects(rect))
                    {
                        flag = true;
                        if (self.velocity.Y > 1f)
                            self.velocity.Y = 1f;
                        NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, self.whoAmI);
                    }

                    j++;
                }
            }
        }
        else
            flag = true;

        if (!flag)
            return;

        num = (int)((vector.X - 16f - 8f) / 16f);
        num2 = (int)((vector.X + self.width + 16f + 8f) / 16f);
        if (boostedChance >= 8)
        {
            num = (int)(vector.X / 16f);
            num2 = (int)((vector.X + self.width) / 16f);

            num3 = (int)((vector.Y + 8f) / 16f);
            int num4 = (int)((vector.Y + self.height + 8f) / 16f);
            for (int k = num; k <= num2; k++)
            {
                for (int l = num3; l <= num4; l++)
                {
                    if (Main.tile[k, l].HasUnactuatedTile && TileID.Sets.CrackedBricks[Main.tile[k, l].TileType])
                    {
                        WorldGen.KillTile(k, l);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, k, l);
                    }
                }
            }
        }
        else
        {
            for (int k = num; k <= num2; k++)
            {
                for (int l = num3; l <= num3 + 2; l++)
                {
                    if (Main.tile[k, l].HasUnactuatedTile && !WorldGen.SolidTile(k, l - 1) &&
                        TileID.Sets.CrackedBricks[Main.tile[k, l].TileType])
                    {
                        WorldGen.KillTile(k, l);
                        if (Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 20, k, l);
                    }
                }
            }
        }
    }
}
