using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WgMod.Common.Players;

namespace WgMod.Common.Systems;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.anolivewine)]
public class WgMannequinSystem : ModSystem
{
    public static WgMannequinSystem Instance => ModContent.GetInstance<WgMannequinSystem>();

    static FieldInfo _dollPlayer;
    static Dictionary<Point, int> _displayDollTileEntityPositions;

    static Mod _mod;
    static Asset<Texture2D> _weightMinus;
    static Asset<Texture2D> _weightPlus;

    public override void SetStaticDefaults()
    {
        _dollPlayer = typeof(TEDisplayDoll).GetField(nameof(_dollPlayer), BindingFlags.Instance | BindingFlags.NonPublic);
        FieldInfo fieldInfo = typeof(TileDrawing).GetField(nameof(_displayDollTileEntityPositions), BindingFlags.Instance | BindingFlags.NonPublic);
        _displayDollTileEntityPositions = (Dictionary<Point, int>)fieldInfo.GetValue(Main.instance.TilesRenderer);
    }

    public override void Load()
    {
        _mod = Mod;
        _weightMinus = Mod.Assets.Request<Texture2D>("Assets/Textures/WeightMinus");
        _weightPlus = Mod.Assets.Request<Texture2D>("Assets/Textures/WeightPlus");

        On_TEDisplayDoll.SaveData += OnSaveData;
        On_TEDisplayDoll.LoadData += OnLoadData;
        On_TEDisplayDoll.NetSend += OnNetSend;
        On_TEDisplayDoll.NetReceive += OnNetReceive;
        On_TEDisplayDoll.OnInventoryDraw += OnInventoryDraw;
    }

    public override void Unload()
    {
        On_TEDisplayDoll.SaveData -= OnSaveData;
        On_TEDisplayDoll.LoadData -= OnLoadData;
        On_TEDisplayDoll.NetSend -= OnNetSend;
        On_TEDisplayDoll.NetReceive -= OnNetReceive;
        On_TEDisplayDoll.OnInventoryDraw -= OnInventoryDraw;
    }

    public override void PostUpdateEverything()
    {
        foreach (KeyValuePair<Point, int> pair in _displayDollTileEntityPositions)
        {
            if (pair.Value != -1 && TileEntity.ByPosition.TryGetValue(new Point16(pair.Key.X, pair.Key.Y), out TileEntity tileEntity))
            {
                TEDisplayDoll doll = (TEDisplayDoll)tileEntity;
                if (GetPlayer(doll).TryGetModPlayer(out WgPlayer wg))
                {
                    wg.Player.gfxOffY = 0f;
                    wg.PreUpdateVisuals();
                    wg.PostUpdateVisuals();
                }
            }
        }
    }

    public static Player GetPlayer(TEDisplayDoll doll)
    {
        return (Player)_dollPlayer.GetValue(doll);
    }

    public static void SetStage(TEDisplayDoll doll, int stage, bool network = true)
    {
        if (!GetPlayer(doll).TryGetModPlayer(out WgPlayer wg))
            return;
        stage = Math.Clamp(stage, 0, WeightStage.Max);
        wg.SetWeightForced(Weight.FromStage(stage) + 10f, false);
        if (network && Main.netMode != NetmodeID.SinglePlayer)
        {
            ModPacket packet = _mod.GetPacket(WgMod.MessageType.MannequinSetStage);
            packet.Write(doll.ID);
            packet.Write((byte)stage);
            packet.Send();
        }
    }

    static void ChangeStage(TEDisplayDoll doll, int delta)
    {
        if (GetPlayer(doll).TryGetModPlayer(out WgPlayer wg))
            SetStage(doll, wg.Weight.GetStage() + delta);
    }

    static void OnSaveData(On_TEDisplayDoll.orig_SaveData orig, TEDisplayDoll self, TagCompound tag)
    {
        orig(self, tag);
        if (GetPlayer(self).TryGetModPlayer(out WgPlayer wg))
            tag["stage"] = wg.Weight.GetStage();
    }

    static void OnLoadData(On_TEDisplayDoll.orig_LoadData orig, TEDisplayDoll self, TagCompound tag)
    {
        orig(self, tag);
        if (tag.TryGet("stage", out int stage))
            SetStage(self, stage, false);
    }

    static void OnNetSend(On_TEDisplayDoll.orig_NetSend orig, TEDisplayDoll self, BinaryWriter writer)
    {
        orig(self, writer);
        writer.Write((byte)GetPlayer(self).Wg().Weight.GetStage());
    }

    static void OnNetReceive(On_TEDisplayDoll.orig_NetReceive orig, TEDisplayDoll self, BinaryReader reader)
    {
        orig(self, reader);
        SetStage(self, reader.ReadByte(), false);
    }

    static void OnInventoryDraw(On_TEDisplayDoll.orig_OnInventoryDraw orig, TEDisplayDoll self, Player player, SpriteBatch spriteBatch)
    {
        orig(self, player, spriteBatch);
        for (int i = 0; i < 2; i++)
        {
            int x = (int)(73f + (i + 0f) * 56f * Main.inventoryScale);
            int y = (int)(Main.instance.invBottom + (2 + 0.5f) * 56f * Main.inventoryScale);
            if (Utils.FloatIntersect(Main.mouseX, Main.mouseY, 0f, 0f, x, y, TextureAssets.InventoryBack.Width() * Main.inventoryScale, TextureAssets.InventoryBack.Height() * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface)
            {
                player.mouseInterface = true;
                bool click = Main.mouseLeftRelease && Main.mouseLeft;
                if (click)
                {
                    if (i == 0)
                        ChangeStage(self, -1);
                    if (i == 1)
                        ChangeStage(self, 1);
                }
            }
            spriteBatch.Draw(TextureAssets.InventoryBack8.Value, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
            if (i == 0)
                spriteBatch.Draw(_weightMinus.Value, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
            if (i == 1)
                spriteBatch.Draw(_weightPlus.Value, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, Main.inventoryScale, SpriteEffects.None, 0f);
        }
    }
}
