using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace WgMod;

public static class PhysicsUtility
{
    public static bool IsTileSolid(Tile tile, bool tileSolidTop = false)
    {
        if (tile == null)
            return false;
        if (tileSolidTop)
            return tile.HasUnactuatedTile && !tile.IsHalfBlock && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
        return tile.HasUnactuatedTile && !tile.IsHalfBlock && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
    }

    public static bool IsTileSolid(int x, int y, bool tileSolidTop = false)
    {
        if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
            return true;
        return IsTileSolid(Main.tile[x, y], tileSolidTop);
    }

    public static bool SolvePenetration(Vector2 position, out Vector2 normal, out float depth, int maxScan = 100)
    {
        Point start = position.ToTileCoordinates();
        if (!IsTileSolid(start.X, start.Y))
        {
            normal = Vector2.Zero;
            depth = 0f;
            return false;
        }

        Span<Point> checkDirs =
        [
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1, 1),
            new Point(-1, 0),
            new Point(-1, -1)
        ];
        List<(Point, Point)> foundPoints = [];
        for (int i = 0; i < maxScan; i++)
        {
            foreach (Point dir in checkDirs)
            {
                Point check = new(start.X + dir.X * i, start.Y + dir.Y * i);
                if (!IsTileSolid(check.X, check.Y))
                    foundPoints.Add((dir, check));
            }
            if (foundPoints.Count > 0)
                break;
        }

        if (foundPoints.Count <= 0)
        {
            normal = Vector2.Zero;
            depth = 0f;
            return false;
        }

        normal = Vector2.Zero;
        depth = float.PositiveInfinity;
        foreach ((Point dir, Point point) in foundPoints)
        {
            Vector2 ndir = Vector2.Normalize(dir.ToVector2());
            Vector2 diff = Vector2.Zero;
            diff.X = (point.X + (dir.X < 0 ? 1 : 0)) * 16f - position.X;
            diff.Y = (point.Y + (dir.Y < 0 ? 1 : 0)) * 16f - position.Y;
            float dist = Vector2.Dot(diff, ndir);
            if (dist < depth)
            {
                normal = ndir;
                depth = dist;
            }
        }
        return true;
    }

    // https://lodev.org/cgtutor/raycasting.html
    public static bool RayIntersectSolid(Vector2 origin, Vector2 dir, float maxDistance, out Vector2 point, out Vector2 normal, bool tileSolidTop = false)
    {
        Point map = origin.ToTileCoordinates();
        Vector2 sideDist = Vector2.Zero;
        Vector2 deltaDist = new(dir.X == 0f ? 1e30f : MathF.Abs(1f / dir.X), dir.Y == 0f ? 1e30f : MathF.Abs(1f / dir.Y));
        Point step = Point.Zero;

        if (dir.X < 0f)
        {
            step.X = -1;
            sideDist.X = (origin.X - map.X * 16f) * deltaDist.X;
        }
        else
        {
            step.X = 1;
            sideDist.X = ((map.X + 1) * 16f - origin.X) * deltaDist.X;
        }

        if (dir.Y < 0f)
        {
            step.Y = -1;
            sideDist.Y = (origin.Y - map.Y * 16f) * deltaDist.Y;
        }
        else
        {
            step.Y = 1;
            sideDist.Y = ((map.Y + 1) * 16f - origin.Y) * deltaDist.Y;
        }

        float distance = 0f;
        while (distance < maxDistance)
        {
            if (sideDist.X < sideDist.Y)
            {
                distance = sideDist.X;
                sideDist.X += deltaDist.X * 16f;
                map.X += step.X;
                normal = new Vector2(-step.X, 0f);
            }
            else
            {
                distance = sideDist.Y;
                sideDist.Y += deltaDist.Y * 16f;
                map.Y += step.Y;
                normal = new Vector2(0f, -step.Y);
            }
            if (distance > maxDistance)
                break;
            if (IsTileSolid(map.X, map.Y, normal.Y < 0f && tileSolidTop))
            {
                point = origin + dir * distance;
                return true;
            }
        }
        point = origin + dir * maxDistance;
        normal = -dir;
        return false;
    }
}
