using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Players;

namespace WgMod.Content.NPCs.Caverns;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class SweetSpirit : ModNPC
{
    public const int FrameCount = 20;
    public const int WanderTime = 8 * 60;

    enum State : byte
    {
        Wandering = 0,
        Positioning,
        Entering,
        Possess
    }

    ref float Timer => ref NPC.ai[3];

    State _state;
    int _frame;

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = FrameCount;
    }

    public override void SetDefaults()
    {
        NPC.width = 28;
        NPC.height = 38;
        NPC.damage = 15;
        NPC.defense = 8;
        NPC.lifeMax = 50;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 60f;
        NPC.aiStyle = NPCAIStyleID.HoveringFighter;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.friendly = false;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
            new FlavorTextBestiaryInfoElement(Mod.GetLocalizationKey("Bestiary." + nameof(SweetSpirit)))
        ]);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return SpawnCondition.Cavern.Chance * 0.05f;
    }

    public override void DrawBehind(int index)
    {
        NPC.hide = true;
        Main.instance.DrawCacheNPCsOverPlayers.Add(index);
    }

    public override void OnSpawn(IEntitySource source)
    {
        Timer = Main.rand.Next(WanderTime - 60, WanderTime + 120 + 1);
        SetState(State.Wandering);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = _frame * frameHeight;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write((byte)_state);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        SetState((State)reader.ReadByte());
    }

    public override void AI()
    {
        switch (_state)
        {
            case State.Wandering:
                NPC.TargetClosest();
                IdleAnimation();
                if (NPC.HasPlayerTarget)
                {
                    Timer--;
                    if (Timer < 0f)
                    {
                        if (Vector2.DistanceSquared(NPC.Center, Main.player[NPC.target].Center) < 100f * 100f)
                            SetState(State.Positioning);
                        else
                            Timer = WanderTime / 4;
                    }
                }
                else
                    Timer = WanderTime;
                break;
            case State.Positioning:
                IdleAnimation();
                if (NPC.HasPlayerTarget)
                {
                    Player player = Main.player[NPC.target];
                    NPC.direction = -player.direction;
                    Vector2 target = GetEnterPosition(player);
                    NPC.velocity = (target - NPC.Center) * 0.2f;
                    if (Vector2.DistanceSquared(NPC.Center, target) < 20f * 20f)
                        SetState(State.Entering);
                }
                else
                    SetState(State.Wandering);
                break;
            case State.Entering:
                if (NPC.HasPlayerTarget)
                {
                    Player player = Main.player[NPC.target];
                    NPC.direction = -player.direction;
                    NPC.velocity = GetEnterPosition(player) - NPC.Center;

                    NPC.frameCounter++;
                    if (NPC.frameCounter > 5)
                    {
                        NPC.frameCounter = 0;
                        if (_frame >= FrameCount - 1)
                            SetState(State.Possess);
                        else
                            _frame++;
                    }
                }
                else
                    SetState(State.Wandering);
                break;
            case State.Possess:
                if (NPC.HasPlayerTarget && Main.player[NPC.target].TryGetModPlayer(out WgPlayer wg))
                {
                    int stage = wg.Weight.GetStage();
                    Mass mass = (Weight.FromStage(stage + 1).Mass - Weight.FromStage(stage).Mass) * 0.5f + 10f;
                    wg.CombatWeightText(wg.AddWeight(mass), false); // Add around half a stage worth of weight
                }
                NPC.life = 0;
                break;
        }
        NPC.spriteDirection = NPC.direction;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 offset = new(0f, -22f);
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center + offset - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        return false;
    }

    void SetState(State state)
    {
        if (_state == state)
            return;
        _state = state;
        switch (_state)
        {
            case State.Wandering:
            case State.Positioning:
                _frame = 0;
                break;
            case State.Entering:
                _frame = 4;
                break;
        }
        NPC.frameCounter = 0;
        NPC.netUpdate = true;
    }

    void IdleAnimation()
    {
        NPC.frameCounter++;
        if (NPC.frameCounter > 10)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= 4;
        }
    }

    static Vector2 GetEnterPosition(Player player)
    {
        return new Vector2(player.Center.X + player.direction * 32f, player.VisualPosition.Y + 31f);
    }
}
