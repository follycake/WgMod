using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class HoseRenderer : ILoadable
{
    public const int PointCount = 16;
    public const int VertexCount = PointCount * 2;
    public const int TriangleCount = (PointCount - 1) * 2;
    public const int IndexCount = TriangleCount * 3;
    public const float BaseRadius = 1.5f;

    static Asset<Texture2D> _white;
    static VertexPositionColorTexture[] _vertexData;
    static short[] _indexData;

    public void Load(Mod mod)
    {
        _white = mod.Assets.Request<Texture2D>("Assets/Textures/White");
        _vertexData = new VertexPositionColorTexture[VertexCount];
        _indexData = new short[IndexCount];
        int j = 0;
        for (int i = 0; i < PointCount - 1; i++)
        {
            _indexData[j++] = (short)i;
            _indexData[j++] = (short)(i + 1);
            _indexData[j++] = (short)(i + PointCount);

            _indexData[j++] = (short)(i + PointCount);
            _indexData[j++] = (short)(i + 1);
            _indexData[j++] = (short)(i + 1 + PointCount);
        }
    }

    public void Unload()
    {
        _vertexData = null;
        _indexData = null;
    }

    public static void SetPoints(ReadOnlySpan<HosePoint> points, Vector2 offset, float radius, Color color)
    {
        if (points.Length != PointCount)
            throw new Exception("Invalid point count");
        for (int i = 0; i < points.Length; i++)
        {
            HosePoint point = points[i];
            Vector2 dir;
            int sign = 1;
            if (i == 0)
            {
                HosePoint next = points[i + 1];
                sign = Math.Sign(next.Position.X - point.Position.X);
                dir = Vector2.UnitX;
            }
            else if (i == points.Length - 1)
            {
                HosePoint prev = points[i - 1];
                sign = Math.Sign(point.Position.X - prev.Position.X);
                dir = Vector2.UnitX;
            }
            else
            {
                HosePoint prev = points[i - 1];
                HosePoint next = points[i + 1];
                dir = prev.Position.DirectionTo(next.Position);
            }
            Vector2 up = new(dir.Y, -dir.X);
            float r = i == 0 ? 0f : radius + point.Thickness;
            _vertexData[i] = new VertexPositionColorTexture(new Vector3(point.Position + up * r * sign + offset, 0f), color, Vector2.Zero);
            _vertexData[i + PointCount] = new VertexPositionColorTexture(new Vector3(point.Position - up * r * sign + offset, 0f), color, Vector2.UnitY);
        }
    }

    public static void Draw(GraphicsDevice device, Texture2D texture = null)
    {
        device.Textures[0] = texture ?? _white.Value;
        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertexData, 0, VertexCount, _indexData, 0, TriangleCount);
    }
}
