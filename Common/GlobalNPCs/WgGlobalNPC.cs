using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WgMod.Common.GlobalNPCs;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
public class WgGlobalNPC : GlobalNPC
{
    public const int StageCount = 2;
    public const int MaxStage = StageCount - 1;
    public const int DaysPerStage = 2;

    public override bool InstancePerEntity => true;

    static readonly Dictionary<int, string> _fatNameLookup = new()
    {
        [NPCID.Dryad] = "Dryad"
    };

    public NPC NPC { get; private set; }
    public int Stage { get; private set; }
    public bool Initialized { get; private set; }

    Asset<Texture2D>[] _stageTextures;
    double _fatTimer;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return _fatNameLookup.ContainsKey(entity.type);
    }

    public void SetStage(int stage)
    {
        stage = Math.Clamp(stage, 0, MaxStage);
        if (Stage != stage)
        {
            Stage = stage;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
        }
    }

    public void Initialize(NPC npc)
    {
        if (Initialized)
            return;
        Initialized = true;
        NPC = npc;
        _fatTimer = 0f;
        _stageTextures = new Asset<Texture2D>[StageCount];
        _stageTextures[0] = TextureAssets.Npc[NPC.type];
        string name = _fatNameLookup[NPC.type];
        for (int i = 1; i < StageCount; i++)
            _stageTextures[i] = Mod.Assets.Request<Texture2D>($"Assets/Textures/WgNPCs/{name}{i}");
    }

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        Initialize(npc);
    }

    public override void AI(NPC npc)
    {
        if (!Initialized)
            Initialize(npc);
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        if (_fatTimer > (Main.dayLength + Main.nightLength) * DaysPerStage)
        {
            SetStage(Stage + 1);
            _fatTimer = 0f;
        }
        else
            _fatTimer += Main.dayRate;
    }

    public override void OnKill(NPC npc)
    {
        _fatTimer = 0f;
        SetStage(0);
    }

    public override void SaveData(NPC npc, TagCompound tag)
    {
        tag[nameof(Stage)] = Stage;
    }

    public override void LoadData(NPC npc, TagCompound tag)
    {
        Initialize(npc);
        if (!tag.TryGet(nameof(Stage), out int stage))
            stage = 0;
        SetStage(stage);
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        binaryWriter.Write((byte)Stage);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        SetStage(binaryReader.ReadByte());
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (Stage <= 0)
            return true;
        DrawTownNPC(npc, spriteBatch, screenPos, drawColor, _stageTextures[Stage].Value);
        return false;
    }

    static void DrawTownNPC(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor, Texture2D texture)
    {
        drawColor = npc.GetNPCColorTintedByBuffs(drawColor);
        Vector2 pos = npc.position - screenPos;
        pos += new Vector2(20f, 28f);
        pos -= new Vector2(11f, 12f);
        int frame = npc.frame.Y / npc.frame.Height;
        Rectangle rect = texture.Frame(1, Main.npcFrameCount[npc.type], 0, frame);
        spriteBatch.Draw(texture, pos, rect, drawColor, npc.rotation, rect.Size() * 0.5f, npc.scale, npc.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
    }
}
