using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Common;

public static class WgPhysics
{
    public static bool Enabled => !WgClientConfig.Instance.DisableAdvancedJiggle && !WgClientConfig.Instance.DisableJiggle;

    public struct Quad(int topLeft, int topRight, int bottomRight, int bottomLeft, Vector2 size)
    {
        // Fixed data
        public int TopLeft = topLeft;
        public int TopRight = topRight;
        public int BottomRight = bottomRight;
        public int BottomLeft = bottomLeft;
        public Vector2 Size = size;

        // Dynamic data
        public Vector2 Center;
        public Vector2 Scale;
        public float Angle;

        public static readonly Vector2[] Frame =
        [
            new Vector2(-0.5f, -0.5f),
            new Vector2(0.5f, -0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(-0.5f, 0.5f)
        ];

        public readonly Vector2 GetFrame(int index, int direction, int gravDir)
        {
            Vector2 frame = Frame[index] * Size;
            frame.X *= direction;
            frame.Y *= gravDir;
            return frame;
        }

        public readonly int GetPoint(int index) => index switch
        {
            0 => TopLeft,
            1 => TopRight,
            2 => BottomRight,
            3 => BottomLeft,
            _ => -1
        };
    }

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

    public sealed class Layer : IDisposable
    {
        public const int GridSize = 8;

        public SpriteSet Set;
        public SpriteSet.Stage StageData;
        public int PhysicsIndex;
        public bool Active;

        public Point[] Points;
        public Spring[] Springs;
        public Quad[] Quads;

        public bool IsEmpty => VertexData.Length <= 0 || IndexData.Length <= 0;
        public VertexPositionColorTexture[] VertexData;
        public short[] IndexData;

        public DynamicVertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public Vector2 AABBMin;
        public Vector2 AABBMax;
        public Vector2 AABBSize => AABBMax - AABBMin;

        public Vector2 OffsetMin;
        public Vector2 OffsetMax;

        int _lastDirection = 1;
        int _lastGravDir = 1;
        Vector2 _basePositionRotated;
        readonly List<Player> _overlappingPlayers = [];

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

        public void Dispose()
        {
            VertexBuffer?.Dispose();
            VertexBuffer = null;
            IndexBuffer?.Dispose();
            IndexBuffer = null;
            Active = false;
        }

        public void Jiggle(float amount)
        {
            Vector2 squish = Set.PhysicsLayers[PhysicsIndex].Squish(1f + amount / 60f);
            Vector2 center = Vector2.Zero;
            foreach (Point point in Points)
                center += point.Position;
            center /= Points.Length;
            foreach (ref Point point in Points.AsSpan())
            {
                Vector2 offset = point.Position - center;
                offset *= squish;
                point.Position = center + offset;
            }
        }

        public bool ShouldBeActive(WgPlayer wg)
        {
            return !IsEmpty && Set.PhysicsLayers[PhysicsIndex].ShouldRender(wg.Player);
        }

        public void Update(WgPlayer wg)
        {
            if (!Active)
            {
                Reset(wg);
                Active = true;
            }

            const float shapeMatchingForce = 1f;
            float immobility = wg.Weight.ClampedImmobility;
            float springForce = float.Lerp(0.8f, 0.4f, immobility);
            float guidanceForce = float.Lerp(0.5f, 0.3f, immobility);

            Span<Point> pointSpan = Points;
            int direction = wg.Player.direction;
            int gravDir = (int)wg.Player.gravDir;
            SpriteSet.LayerType layerType = Set.PhysicsLayers[PhysicsIndex].Type;

            Vector2 drawPosition = CalculateDrawPosition(wg);
            Vector2 rotationCenter = drawPosition + wg.Player.fullRotationOrigin;
            Vector2 flipCenter = wg.Player.Center;
            if (direction != _lastDirection || gravDir != _lastGravDir)
            {
                foreach (ref Point point in pointSpan)
                {
                    Vector2 pos = point.Position.RotatedBy(-wg.Player.fullRotation, rotationCenter) - flipCenter;
                    pos.X *= direction * _lastDirection;
                    pos.Y *= gravDir * _lastGravDir;
                    point.Teleport((flipCenter + pos).RotatedBy(wg.Player.fullRotation, rotationCenter));
                }
            }

            _lastDirection = direction;
            _lastGravDir = gravDir;

            // Verlet integration
            foreach (ref Point point in pointSpan)
            {
                Vector2 velocity = point.Position - point.LastPosition;
                velocity.Y += Player.defaultGravity * 2f * gravDir;
                point.LastPosition = point.Position;
                point.Position += velocity;
            }

            // Springs
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
                    a.Position += dir * (error * 0.5f * force);
                    b.Position -= dir * (error * 0.5f * force);
                }
            }

            // Compute quad data
            foreach (ref Quad quad in Quads.AsSpan())
            {
                quad.Center = Vector2.Zero;
                for (int i = 0; i < 4; i++)
                    quad.Center += pointSpan[quad.GetPoint(i)].Position;
                quad.Center /= 4f;
                Vector2 quadVect = Vector2.Zero;
                for (int i = 0; i < 4; i++)
                {
                    Vector2 frame = quad.GetFrame(i, direction, gravDir);
                    Vector2 pos = pointSpan[quad.GetPoint(i)].Position - quad.Center;
                    float angle = Utility.AngleDifference(frame.ToRotation(), pos.ToRotation());
                    quadVect += new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                }
                quadVect /= 4f;
                quad.Angle = quadVect.ToRotation();

                Vector2 topLeft = pointSpan[quad.TopLeft].Position;
                Vector2 topRight = pointSpan[quad.TopRight].Position;
                Vector2 bottomRight = pointSpan[quad.BottomRight].Position;
                Vector2 bottomLeft = pointSpan[quad.BottomLeft].Position;
                float sizeX = (topLeft.Distance(topRight) + bottomLeft.Distance(bottomRight)) * 0.5f;
                float sizeY = (topLeft.Distance(bottomLeft) + topRight.Distance(bottomRight)) * 0.5f;
                quad.Scale = new Vector2((quad.Size.Y / sizeY + 1f) * 0.5f, (quad.Size.X / sizeX + 1f) * 0.5f);
            }

            // Shape matching
            foreach (Quad quad in Quads)
            {
                for (int i = 0; i < 4; i++)
                {
                    ref Point point = ref pointSpan[quad.GetPoint(i)];
                    Vector2 target = quad.Center + (quad.GetFrame(i, direction, gravDir) * quad.Scale).RotatedBy(quad.Angle);
                    float distance = point.Position.Distance(target);
                    if (distance > 0.01f)
                    {
                        Vector2 dir = point.Position.DirectionTo(target);
                        point.Position += dir * (distance * 0.5f * shapeMatchingForce);
                    }
                }
            }

            // Guidance and pinning
            Vector2 basePosition = CalculateBasePosition(wg, drawPosition);
            _basePositionRotated = basePosition.RotatedBy(wg.Player.fullRotation, drawPosition + wg.Player.fullRotationOrigin);
            foreach (ref Point point in pointSpan)
            {
                float t = Utils.GetLerpValue(OffsetMin.X, OffsetMax.X, point.Offset.X);
                Vector2 offset = point.Offset;
                float maxCurve = layerType switch
                {
                    SpriteSet.LayerType.Breasts => 1.2f,
                    _ => 1.8f
                };
                offset.Y *= float.Lerp(1f, maxCurve, t * t * wg._softSquishRight);
                offset.Y *= float.Lerp(1f, maxCurve, (1f - t) * (1f - t) * wg._softSquishLeft * wg._softSquishLeft);
                Vector2 target = CalculateFromOffset(wg, drawPosition, basePosition, offset);
                if (point.Pinned)
                {
                    point.Teleport(target);
                    continue;
                }
                if (float.IsNaN(point.Position.X) || float.IsNaN(point.Position.Y)) // Sometimes the simulation gets funky
                {
                    point.Teleport(target);
                    continue;
                }
                float distance = point.Position.Distance(target);
                if (distance > 0.01f)
                {
                    Vector2 dir = point.Position.DirectionTo(target);
                    point.Position += dir * (distance * 0.5f * guidanceForce);
                }
            }

            // Collision
            bool tileSolidTop = layerType == SpriteSet.LayerType.Legs;
            Vector2 center = drawPosition - new Vector2(0f, wg.Player.gfxOffY);
            if (tileSolidTop)
            {
                center.X += wg.Player.width * 0.5f;
                center.Y += wg.Player.height - 16;
            }
            else
                center += wg.Player.Size * 0.5f;
            foreach (ref Point point in pointSpan)
            {
                Vector2 diff = point.Position - center;
                Vector2 dir = Vector2.Normalize(diff);
                float dist = diff.Length();
                if (PhysicsUtility.RayIntersectSolid(center, dir, dist, out Vector2 intersection, out Vector2 normal, tileSolidTop))
                    point.Position = point.Position - normal * Vector2.Dot(normal, point.Position) + normal * Vector2.Dot(normal, intersection);
                foreach (Player player in _overlappingPlayers)
                {
                    float collision = 0f;
                    if (Collision.CheckAABBvLineCollision(player.position, player.Size, center, point.Position, 1f, ref collision))
                        point.Position = center + dir * collision;
                }
            }

            // Bounding box
            AABBMin = new(float.PositiveInfinity, float.PositiveInfinity);
            AABBMax = new(float.NegativeInfinity, float.NegativeInfinity);
            foreach (ref Point point in pointSpan)
            {
                AABBMin = Vector2.Min(AABBMin, point.Position);
                AABBMax = Vector2.Max(AABBMax, point.Position);
            }
            AABBMin -= Vector2.One;
            AABBMax += Vector2.One;
        }

        public void FindOverlappingPlayers(int ignore)
        {
            _overlappingPlayers.Clear();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (i == ignore)
                    continue;
                Player player = Main.player[i];
                if (player.active && Collision.CheckAABBvAABBCollision(AABBMin, AABBSize, player.position, player.Size))
                    _overlappingPlayers.Add(player);
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

        public void Draw(GraphicsDevice device, DrawData drawData)
        {
            if (IsEmpty)
                return;

            Vector2 offset = drawData.position - _basePositionRotated;
            UpdateVertexData(offset, drawData.sourceRect.Value, drawData.texture.Size(), drawData.color);
            device.Textures[0] = drawData.texture;

            VertexBufferBinding[] prevVertexBuffers = device.GetVertexBuffers();
            IndexBuffer prevIndices = device.Indices;

            if (IndexBuffer == null || IndexBuffer.GraphicsDevice != device)
            {
                IndexBuffer?.Dispose();
                IndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, IndexData.Length, BufferUsage.WriteOnly);
                IndexBuffer.SetData(IndexData);
            }
            if (VertexBuffer == null || VertexBuffer.GraphicsDevice != device)
            {
                VertexBuffer?.Dispose();
                VertexBuffer = new DynamicVertexBuffer(device, typeof(VertexPositionColorTexture), VertexData.Length, BufferUsage.WriteOnly);
            }
            VertexBuffer.SetData(VertexData);

            device.SetVertexBuffer(VertexBuffer);
            device.Indices = IndexBuffer;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexData.Length, 0, IndexData.Length / 3);
            //DrawDebug(device);

            device.SetVertexBuffers(prevVertexBuffers);
            device.Indices = prevIndices;
        }

        public void DrawDebug(GraphicsDevice device)
        {
            Vector2 offset = -Main.screenPosition;
            foreach (Spring spring in Springs)
                device.DrawLine(Points[spring.A].Position + offset, Points[spring.B].Position + offset, Color.Red);
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

        public static Vector2 CalculateFromOffset(WgPlayer wg, Vector2 drawPosition, Vector2 basePosition, Vector2 offset)
        {
            offset.X *= wg.Player.direction;
            offset.Y *= wg.Player.gravDir;
            basePosition += offset;
            basePosition = basePosition.RotatedBy(wg.Player.fullRotation, drawPosition + wg.Player.fullRotationOrigin);
            return basePosition;
        }
    }

    public static bool IsEnabled(WgPlayer wg)
    {
        return wg._physicsLayers != null && wg._physicsLayers.Count > 0;
    }

    public static bool Dispose(WgPlayer wg)
    {
        if (IsEnabled(wg))
        {
            foreach (Layer layer in wg._physicsLayers)
                layer.Dispose();
            wg._physicsLayers.Clear();
            return true;
        }
        return false;
    }

    public static void Clear(WgPlayer wg)
    {
        Dispose(wg);
        wg._physicsLayers = null;
        wg._physicsDrawOverride = null;
    }

    public static bool Setup(WgPlayer wg)
    {
        if (!Enabled || wg.Player.isDisplayDollOrInanimate)
        {
            Clear(wg);
            return false;
        }
        int stage = wg.Weight.GetStage();
        if (stage <= 0)
        {
            Clear(wg);
            return false;
        }
        if (wg._physicsLayers != null)
            Dispose(wg);
        else
            wg._physicsLayers = [];
        wg._physicsDrawOverride ??= [];
        wg._physicsDrawOverride.Clear();
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
            layer.OffsetMin = new(float.PositiveInfinity, float.PositiveInfinity);
            layer.OffsetMax = new(float.NegativeInfinity, float.NegativeInfinity);
            int CreatePoint(int x, int y)
            {
                Vector2 offset = new(x - w * 0.5f, y - h * 0.5f);
                int existing = points.FindIndex(p => p.Offset == offset);
                if (existing >= 0)
                    return existing;
                layer.OffsetMin = Vector2.Min(layer.OffsetMin, offset);
                layer.OffsetMax = Vector2.Max(layer.OffsetMax, offset);
                points.Add(new Point(Layer.CalculateFromOffset(wg, drawPosition, position, offset))
                {
                    Offset = offset,
                    UV = new Vector2(x / (w - 1f), y / (h - 1f))
                });
                return points.Count - 1;
            }

            List<short> indexData = [];
            List<Quad> quads = [];
            Dictionary<(int, int), int> quadMap = [];
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

                    int quadX = x / Layer.GridSize;
                    int quadY = y / Layer.GridSize;
                    quadMap.Add((quadX, quadY), quads.Count);
                    quads.Add(new Quad(a, b, c, d, new Vector2(xEnd - x, yEnd - y)));

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

            bool ScanForQuads(int x, int y, int radius)
            {
                for (int delta = -radius; delta <= radius; delta++)
                {
                    if (quadMap.ContainsKey((x + delta, y)))
                        return true;
                }
                return false;
            }

            Point[] pointsArray = [.. points];
            foreach (KeyValuePair<(int, int), int> pair in quadMap)
            {
                var (x, y) = pair.Key;
                Quad quad = quads[pair.Value];
                if (!ScanForQuads(x, y - 1, 2))
                {
                    pointsArray[quad.TopLeft].Pinned = true;
                    pointsArray[quad.TopRight].Pinned = true;
                    pointsArray[quad.BottomLeft].Pinned = true;
                    pointsArray[quad.BottomRight].Pinned = true;
                }
                if (!quadMap.ContainsKey((x, y - 1)))
                    Join(quad.TopLeft, quad.TopRight, -1f);
                if (!quadMap.ContainsKey((x + 1, y)))
                    Join(quad.TopRight, quad.BottomRight, -1f);
                if (!quadMap.ContainsKey((x, y + 1)))
                    Join(quad.BottomLeft, quad.BottomRight, -1f);
                if (!quadMap.ContainsKey((x - 1, y)))
                    Join(quad.TopLeft, quad.BottomLeft, -1f);
            }
            foreach (Quad quad in quads)
            {
                Join(quad.TopLeft, quad.TopRight);
                Join(quad.TopLeft, quad.BottomLeft);
                Join(quad.TopRight, quad.BottomRight);
                Join(quad.BottomLeft, quad.BottomRight);

                Join(quad.TopLeft, quad.BottomRight);
                Join(quad.TopRight, quad.BottomLeft);
            }

            layer.PhysicsIndex = wg._physicsLayers.Count;
            layer.Points = pointsArray;
            layer.Springs = [.. springs];
            layer.Quads = [.. quads];
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
            Clear(wg);
            return;
        }
        if (!IsEnabled(wg) && !Setup(wg))
            return;
        foreach (Layer layer in wg._physicsLayers)
        {
            if (layer.ShouldBeActive(wg))
                layer.Update(wg);
            else
                layer.Active = false;
        }
        foreach (Layer layer in wg._physicsLayers)
        {
            if (layer.Active)
                layer.FindOverlappingPlayers(wg.Player.whoAmI);
        }
    }

    public static void Jiggle(WgPlayer wg, float amount)
    {
        if (!IsEnabled(wg))
            return;
        foreach (Layer layer in wg._physicsLayers)
        {
            if (layer.Active)
                layer.Jiggle(amount);
        }
    }
}
