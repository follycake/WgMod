using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgPhysics
{
    public static bool Enabled => !WgClientConfig.Instance.DisableAdvancedJiggle && !WgClientConfig.Instance.DisableJiggle;

    public record struct Quad(int TopLeft, int TopRight, int BottomRight, int BottomLeft);

    public struct Point(Vector2 position)
    {
        public Vector2 Position = position;
        public Vector2 LastPosition = position;

        public Vector2 Offset;
        public Vector2 UV;
        public bool Pinned;

        public void Teleport(Vector2 position)
        {
            Position = position;
            LastPosition = position;
        }
    }

    public struct Spring(int a, int b, float restDistance, float strengh = 1f)
    {
        public int A = a;
        public int B = b;
        public float RestDistance = restDistance;
        public float Strength = strengh;
        public readonly bool Rigid => Strength < 0f;
    }

    public class Layer
    {
        public const int GridSize = 8;

        public SpriteSet Set;
        public SpriteSet.Stage StageData;
        public int PhysicsIndex;
        public bool Active;

        public Point[] Points;
        public Spring[] Springs;

        public VertexPositionColorTexture[] VertexData;
        public short[] IndexData;

        public HashSet<int> DrawDataOverrides = [];

        int _lastDirection = 1;
        int _lastGravDir = 1;

        public void Setup(WgPlayer wg)
        {
            _lastDirection = wg.Player.direction;
            _lastGravDir = (int)wg.Player.gravDir;
            Active = false;
        }

        public void Reset(WgPlayer wg)
        {
            _lastDirection = wg.Player.direction;
            _lastGravDir = (int)wg.Player.gravDir;
            Vector2 drawPosition = CalculateDrawPosition(wg);
            Vector2 position = CalculateBasePosition(wg, drawPosition);
            foreach (ref Point point in Points.AsSpan())
                point.Teleport(CalculateFromOffset(wg, drawPosition, position, point.Offset));
        }

        public void Update(WgPlayer wg)
        {
            const float springForce = 0.3f;
            const float guidanceForce = 0.3f;

            if (!Active)
            {
                Reset(wg);
                Active = true;
            }

            Span<Point> span = Points;
            int direction = wg.Player.direction;
            int gravDir = (int)wg.Player.gravDir;

            Vector2 drawPosition = CalculateDrawPosition(wg);
            Vector2 rotationCenter = drawPosition + wg.Player.fullRotationOrigin;
            Vector2 flipCenter = wg.Player.Center;
            if (direction != _lastDirection || gravDir != _lastGravDir)
            {
                foreach (ref Point point in span)
                {
                    Vector2 pos = point.Position.RotatedBy(-wg.Player.fullRotation, rotationCenter) - flipCenter;
                    pos.X *= direction * _lastDirection;
                    pos.Y *= gravDir * _lastGravDir;
                    point.Teleport((flipCenter + pos).RotatedBy(wg.Player.fullRotation, rotationCenter));
                }
            }

            _lastDirection = direction;
            _lastGravDir = gravDir;

            foreach (ref Point point in span)
            {
                Vector2 velocity = point.Position - point.LastPosition;
                velocity.Y += Player.defaultGravity * 2f * gravDir;
                point.LastPosition = point.Position;
                point.Position += velocity;
            }

            foreach (Spring spring in Springs)
            {
                ref Point a = ref Points[spring.A];
                ref Point b = ref Points[spring.B];
                float distance = a.Position.Distance(b.Position);
                float error = distance - spring.RestDistance;
                if (MathF.Abs(error) > 0.01f)
                {
                    Vector2 dir = a.Position.DirectionTo(b.Position);
                    float force = spring.Rigid ? 1f : (springForce * spring.Strength);
                    a.Position += dir * error * 0.5f * force;
                    b.Position -= dir * error * 0.5f * force;
                }
            }

            Vector2 position = CalculateBasePosition(wg, drawPosition);
            foreach (ref Point point in span)
            {
                Vector2 target = CalculateFromOffset(wg, drawPosition, position, point.Offset);
                if (point.Pinned)
                {
                    point.Teleport(target);
                    continue;
                }
                float distance = point.Position.Distance(target);
                if (distance > 0.01f)
                {
                    Vector2 dir = point.Position.DirectionTo(target);
                    point.Position += dir * distance * 0.5f * guidanceForce;
                }
            }

            Vector2 center = drawPosition + wg.Player.Size * 0.5f;
            foreach (ref Point point in span)
            {
                Vector2 diff = point.Position - center;
                Vector2 dir = Vector2.Normalize(diff);
                float dist = diff.Length();
                if (PhysicsUtility.RayIntersectSolid(center, dir, dist, out Vector2 p, out _))
                    point.Position = p;
                //if (PhysicsUtility.SolvePenetration(point.Position, out Vector2 normal, out float depth))
                //    point.Position += normal * depth;
            }
        }

        public void UpdateVertexData(Vector2 offset, Rectangle sourceRect, Vector2 textureSize, Color tint)
        {
            Vector2 start = sourceRect.Location.ToVector2() / textureSize;
            Vector2 size = sourceRect.Size() / textureSize;
            for (int i = 0; i < Points.Length; i++)
            {
                Point point = Points[i];
                VertexData[i] = new VertexPositionColorTexture(new Vector3(point.Position + offset, 0f), tint, start + size * point.UV);
            }
        }

        public void Draw(GraphicsDevice device, Texture2D texture, Rectangle sourceRect, Color tint)
        {
            UpdateVertexData(-Main.screenPosition, sourceRect, texture.Size(), tint);
            device.Textures[0] = texture;
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexData, 0, VertexData.Length, IndexData, 0, IndexData.Length / 3);
            //layer.DrawDebug(device);
        }

        public void DrawDebug(GraphicsDevice device)
        {
            Vector2 offset = -Main.screenPosition;
            foreach (Spring spring in Springs)
            {
                if (spring.Rigid)
                    device.DrawLine(Points[spring.A].Position + offset, Points[spring.B].Position + offset, Color.Red);
            }
        }

        public static Vector2 CalculateDrawPosition(WgPlayer wg)
        {
            return WgArmor.GetDrawPosition(wg.Player) + new Vector2(0f, wg.Player.gfxOffY);
        }

        public Vector2 CalculateBasePosition(WgPlayer wg, Vector2 drawPosition)
        {
            Vector2 position = new Vector2((int)(drawPosition.X - wg.Player.bodyFrame.Width / 2 + wg.Player.width / 2), (int)(drawPosition.Y + wg.Player.height - wg.Player.bodyFrame.Height)) + wg.Player.bodyPosition + new Vector2(wg.Player.bodyFrame.Width / 2, wg.Player.bodyFrame.Height / 2);
            position += SpriteSet.GetOffset(Set, StageData, wg.Player.direction, wg.Player.gravDir);
            Set.PhysicsLayers[PhysicsIndex].Animate(wg, position, 1f, 1f, out Vector2 pos, out _);
            return pos;
        }

        public static Vector2 CalculateFromOffset(WgPlayer wg, Vector2 drawPosition, Vector2 position, Vector2 offset)
        {
            offset.X *= wg.Player.direction;
            offset.Y *= wg.Player.gravDir;
            position += offset;
            position = position.RotatedBy(wg.Player.fullRotation, drawPosition + wg.Player.fullRotationOrigin);
            return position;
        }
    }

    public static bool Setup(WgPlayer wg)
    {
        if (!Enabled || wg.Player.isDisplayDollOrInanimate)
        {
            wg._physicsLayers = null;
            return false;
        }
        wg._physicsLayers ??= [];
        wg._physicsLayers.Clear();
        int stage = wg.Weight.GetStage();
        if (stage <= 0)
            return true;
        SpriteSet.Stage stageData = SpriteSet.GetStage(stage, out SpriteSet set);
        foreach (SpriteSet.Layer spriteLayer in set.PhysicsLayers)
        {
            if (!spriteLayer.Texture.IsLoaded)
                spriteLayer.Texture.Wait();

            Rectangle frame = spriteLayer.Frame(set, stageData);
            int w = frame.Width;
            int h = frame.Height;
            int totalW = spriteLayer.Texture.Width();
            int totalH = spriteLayer.Texture.Height();
            Color[] data = new Color[w * h];
            spriteLayer.Texture.Value.GetData(0, frame, data, 0, data.Length);

            int shrunkW = w;
            int shrunkH = h;

            void ShrinkX()
            {
                for (int xCheck = w - 1; xCheck >= 0; xCheck--)
                {
                    for (int yCheck = 0; yCheck < h; yCheck++)
                    {
                        if (data[xCheck + yCheck * w].A > 100)
                            return;
                    }
                    shrunkW--;
                }
            }

            void ShrinkY()
            {
                for (int yCheck = h - 1; yCheck >= 0; yCheck--)
                {
                    for (int xCheck = 0; xCheck < shrunkW; xCheck++)
                    {
                        if (data[xCheck + yCheck * w].A > 100)
                            return;
                    }
                    shrunkH--;
                }
            }

            ShrinkX();
            ShrinkY();

            bool CheckValidQuad(int x, int y)
            {
                for (int my = 0; my < Layer.GridSize; my++)
                {
                    for (int mx = 0; mx < Layer.GridSize; mx++)
                    {
                        if (data[Math.Min(x + mx, w - 1) + Math.Min(y + my, h - 1) * w].A > 100)
                            return true;
                    }
                }
                return false;
            }

            Layer layer = new() { Set = set, StageData = stageData };
            Vector2 drawPosition = Layer.CalculateDrawPosition(wg);
            Vector2 position = layer.CalculateBasePosition(wg, drawPosition);

            List<Point> points = [];
            int CreatePoint(int x, int y)
            {
                Vector2 offset = new(x - w * 0.5f, y - h * 0.5f);
                int existing = points.FindIndex(p => p.Offset == offset);
                if (existing >= 0)
                    return existing;
                points.Add(new Point(Layer.CalculateFromOffset(wg, drawPosition, position, offset))
                {
                    Offset = offset,
                    UV = new Vector2(x / (w - 1f), y / (h - 1f))
                });
                return points.Count - 1;
            }

            List<short> indexData = [];
            Dictionary<(int, int), Quad> quads = [];
            for (int y = 0; y < h; y += Layer.GridSize)
            {
                for (int x = 0; x < w; x += Layer.GridSize)
                {
                    if (!CheckValidQuad(x, y))
                        continue;
                    int xEnd = Math.Min(x + Layer.GridSize, shrunkW - 1);
                    int yEnd = Math.Min(y + Layer.GridSize, shrunkH - 1);

                    int a = CreatePoint(x, y);
                    int b = CreatePoint(xEnd, y);
                    int c = CreatePoint(xEnd, yEnd);
                    int d = CreatePoint(x, yEnd);
                    quads.Add((x / Layer.GridSize, y / Layer.GridSize), new Quad(a, b, c, d));

                    // Front
                    indexData.Add((short)a);
                    indexData.Add((short)b);
                    indexData.Add((short)c);

                    indexData.Add((short)c);
                    indexData.Add((short)d);
                    indexData.Add((short)a);

                    // Back
                    indexData.Add((short)c);
                    indexData.Add((short)b);
                    indexData.Add((short)a);

                    indexData.Add((short)a);
                    indexData.Add((short)d);
                    indexData.Add((short)c);
                }
            }

            List<Spring> springs = [];
            HashSet<(int, int)> usedPairs = [];
            void Join(int a, int b, float strength = 1f)
            {
                if (a == b || usedPairs.Contains((a, b)))
                    return;
                usedPairs.Add((a, b));
                usedPairs.Add((b, a));
                springs.Add(new Spring(a, b, Vector2.Distance(points[a].Position, points[b].Position), strength));
            }

            Point[] pointsArray = [.. points];
            foreach (KeyValuePair<(int, int), Quad> pair in quads)
            {
                var (x, y) = pair.Key;
                Quad quad = pair.Value;
                if (!quads.ContainsKey((x, y - 1)) && !quads.ContainsKey((x - 1, y - 1)))
                {
                    pointsArray[quad.TopLeft].Pinned = true;
                    pointsArray[quad.TopRight].Pinned = true;
                    pointsArray[quad.BottomLeft].Pinned = true;
                    pointsArray[quad.BottomRight].Pinned = true;
                }
                if (!quads.ContainsKey((x, y - 1)))
                    Join(quad.TopLeft, quad.TopRight, -1f);
                if (!quads.ContainsKey((x + 1, y)))
                    Join(quad.TopRight, quad.BottomRight, -1f);
                if (!quads.ContainsKey((x, y + 1)))
                    Join(quad.BottomLeft, quad.BottomRight, -1f);
                if (!quads.ContainsKey((x - 1, y)))
                    Join(quad.TopLeft, quad.BottomLeft, -1f);
            }
            foreach (Quad quad in quads.Values)
            {
                Join(quad.TopLeft, quad.TopRight);
                Join(quad.TopLeft, quad.BottomLeft);
                Join(quad.TopRight, quad.BottomRight);
                Join(quad.BottomLeft, quad.BottomRight);

                Join(quad.TopLeft, quad.BottomRight);
                Join(quad.TopRight, quad.BottomLeft);
            }
            for (int a = 0; a < points.Count - 1; a++)
            {
                for (int b = a + 1; b < points.Count; b++)
                    Join(a, b, 0.01f);
            }

            layer.PhysicsIndex = wg._physicsLayers.Count;
            layer.Points = pointsArray;
            layer.Springs = [.. springs];
            layer.VertexData = new VertexPositionColorTexture[points.Count];
            layer.IndexData = [.. indexData];
            wg._physicsLayers.Add(layer);
        }
        foreach (Layer layer in wg._physicsLayers)
            layer.Setup(wg);
        return true;
    }

    public static void Update(WgPlayer wg)
    {
        if (!Enabled)
        {
            wg._physicsLayers = null;
            return;
        }
        if (wg._physicsLayers == null && !Setup(wg))
            return;
        foreach (Layer layer in wg._physicsLayers)
        {
            if (layer.Set.PhysicsLayers[layer.PhysicsIndex].ShouldRender(wg.Player))
                layer.Update(wg);
            else
                layer.Active = false;
        }
    }
}
