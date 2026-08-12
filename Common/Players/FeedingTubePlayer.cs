using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.TileEntities;

namespace WgMod.Common.Players;

public struct HosePoint(Vector2 position)
{
    public Vector2 Position = position;
    public Vector2 LastPosition = position;
    public float Thickness;

    public void Reset(Vector2 position)
    {
        Teleport(position);
        Thickness = 0f;
    }

    public void Teleport(Vector2 position)
    {
        Position = position;
        LastPosition = position;
    }
}

public class FeedingTubePlayer : ModPlayer
{
    public const float MaxDistance = 16f * 8f;
    public const float PointDistance = MaxDistance / HoseRenderer.PointCount;
    public const int GulpTime = 60;

    public bool Connected => _tube != null;

    TEFeedingTube _tube;
    HosePoint[] _points;
    int _gulpTimer;
    bool _gulpLava;

    static Asset<Texture2D> _hoseTexture;

    public override void Load()
    {
        _hoseTexture = Mod.Assets.Request<Texture2D>("Assets/Textures/Hose");
        On_LegacyPlayerRenderer.DrawPlayerFull += DrawPlayerFull;
    }

    public override void Unload()
    {
        On_LegacyPlayerRenderer.DrawPlayerFull -= DrawPlayerFull;
    }

    public void Connect(TEFeedingTube tube)
    {
        if (_tube == tube)
            return;

        if (Main.netMode != NetmodeID.MultiplayerClient)
            _tube?.SetFeedeeServer(null);
        _tube = tube;
        SoundEngine.PlaySound(SoundID.Grab, Player.Center);

        if (_tube == null)
            return;

        if (Main.netMode != NetmodeID.MultiplayerClient)
            _tube.SetFeedeeServer(this);
        _gulpTimer = 0;
        _gulpLava = false;
        _points ??= new HosePoint[HoseRenderer.PointCount];

        Vector2 mouthPosition = GetMouthPosition();
        Vector2 tubePosition = GetTubePosition();
        for (int i = 0; i < _points.Length; i++)
            _points[i].Reset(Vector2.Lerp(mouthPosition, tubePosition, i / (_points.Length - 1f)));
    }

    public override void PostUpdate()
    {
        if (!Connected)
            return;

        if (!_tube.IsTileValidForEntity(_tube.Position.X, _tube.Position.Y))
        {
            Connect(null);
            return;
        }

        if (_gulpLava)
            Player.TouchLava();

        Vector2 mouthPosition = GetMouthPosition();
        Vector2 tubePosition = GetTubePosition();

        float gulpPos = float.Lerp(HoseRenderer.PointCount - 1f, 0f, _gulpTimer / (GulpTime - 4f));
        if (_tube.IsEmpty)
        {
            _gulpTimer = 0;
            _gulpLava = false;
            gulpPos = -10f;
        }
        else
        {
            if (_gulpTimer < GulpTime)
                _gulpTimer++;
            else
            {
                _gulpTimer = 0;
                SoundEngine.PlaySound(WgSounds.Gulp, Player.Center);
                if (Player.whoAmI == Main.myPlayer)
                {
                    int amount = _tube.AddLiquid(_tube.LiquidType, -1);
                    if (amount != 0)
                    {
                        TEFeedingTube.FluidInfo fluidInfo = TEFeedingTube.FluidTable[_tube.LiquidType];
                        Player.Wg().AddStomach(fluidInfo.Gain);
                        Player.PutItemInInventoryFromItemUsage(ItemID.EmptyBucket);
                        if (_tube.LiquidType == LiquidID.Lava)
                            _gulpLava = true;
                    }
                }
            }
        }

        for (int i = 0; i < _points.Length; i++)
        {
            ref HosePoint point = ref _points[i];
            float x = i - gulpPos;
            point.Thickness = MathF.Exp(-x * x) * 4f;

            Vector2 velocity = point.Position - point.LastPosition;
            point.LastPosition = point.Position;
            point.Position += velocity;
            point.Position.Y += Player.defaultGravity;
        }

        for (int iteration = 0; iteration < 2; iteration++)
        {
            for (int i = 0; i < _points.Length - 1; i++)
            {
                ref HosePoint a = ref _points[i];
                ref HosePoint b = ref _points[i + 1];
                float dist = Vector2.Distance(a.Position, b.Position);
                float error = dist - PointDistance;
                if (MathF.Abs(error) > 0.01f)
                {
                    Vector2 dir = a.Position.DirectionTo(b.Position);
                    a.Position += dir * error * 0.5f;
                    b.Position -= dir * error * 0.5f;
                }
            }
            _points[0].Teleport(mouthPosition);
            _points[^1].Teleport(tubePosition);
        }

        if (Player.dead || tubePosition.DistanceSQ(mouthPosition) > MaxDistance * MaxDistance * 4f)
            Connect(null);
    }

    static void DrawPlayerFull(On_LegacyPlayerRenderer.orig_DrawPlayerFull orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer)
    {
        orig(self, camera, drawPlayer);
        if (drawPlayer.TryGetModPlayer(out FeedingTubePlayer fp) && fp.Connected)
        {
            camera.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, camera.Sampler, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);
            float r = _hoseTexture.Height() * 0.5f;
            HoseRenderer.SetPoints(fp._points, -Main.screenPosition, r + 2f, new Color(45, 46, 77));
            HoseRenderer.Draw(camera.SpriteBatch.GraphicsDevice);
            HoseRenderer.SetPoints(fp._points, -Main.screenPosition, r, Color.White);
            HoseRenderer.Draw(camera.SpriteBatch.GraphicsDevice, _hoseTexture.Value);
            camera.SpriteBatch.End();
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket(WgMod.MessageType.FeedingTubePlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write(_tube != null ? _tube.ID : -1);
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        int id = reader.ReadInt32();
        if (id >= 0)
            Connect((TEFeedingTube)TileEntity.ByID[id]);
        else
            Connect(null);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        FeedingTubePlayer clone = (FeedingTubePlayer)targetCopy;
        clone.Connect(_tube);
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        FeedingTubePlayer clone = (FeedingTubePlayer)clientPlayer;
        if (_tube != clone._tube)
            SyncPlayer(-1, Main.myPlayer, false);
    }

    Vector2 GetMouthPosition()
    {
        return Player.MouthPosition.Value + new Vector2(0f, Player.Wg()._addedGfxOffY);
    }

    Vector2 GetTubePosition()
    {
        if (!Connected)
            return GetMouthPosition();
        return _tube.Position.ToWorldCoordinates(0f);
    }
}
