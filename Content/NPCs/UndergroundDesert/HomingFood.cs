using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.NPCs.UndergroundDesert;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Idea, Contributor.haydumbb)]
public class HomingFood : ModNPC
{
    static readonly int[] _items =
    [
        ItemID.ChristmasPudding,
        ItemID.GingerbreadCookie,
        ItemID.RoastedBird,
        ItemID.MonsterLasagna,
        ItemID.BananaSplit,
        ItemID.Fries,
        ItemID.Burger,
        ItemID.Pizza,
        ItemID.IceCream,
        ItemID.Hotdog,
        ItemID.Milkshake
    ];

    int _itemIndex;
    int _itemId;

    public override void SetDefaults()
    {
        NPC.width = 22;
        NPC.height = 22;
        NPC.damage = 10;
        NPC.defense = 14;
        NPC.lifeMax = 30;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.value = 0f;
        NPC.knockBackResist = 0f;
        NPC.aiStyle = NPCAIStyleID.CursedSkull;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.friendly = false;

        AIType = NPCID.CursedSkull;
        _itemIndex = 0;
        _itemId = _items[_itemIndex];
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,
            new FlavorTextBestiaryInfoElement(Mod.GetLocalizationKey("Bestiary." + nameof(HomingFood)))
        ]);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return SpawnCondition.DesertCave.Chance * 0.1f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        _itemIndex = Main.rand.Next(_items.Length);
        _itemId = _items[_itemIndex];
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(_itemIndex);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        int index = reader.ReadInt32();
        if (_itemIndex != index)
        {
            _itemIndex = index;
            _itemId = _items[_itemIndex];
        }
    }

    public override void PostAI()
    {
        if (NPC.HasPlayerTarget)
        {
            Player player = Main.player[NPC.target];
            if (NPC.getRect().Intersects(player.getRect()))
            {
                player.AddBuff(ModContent.BuffType<ForceFed>(), (int)(ForceFed.TicksPerCycle * 1.5f));
                player.AddBuff(BuffID.WellFed, 60 * 4);
                SoundEngine.PlaySound(SoundID.Item2, NPC.Center);
                NPC.life = 0;
            }
        }
        Lighting.AddLight(NPC.Center, Color.Purple.ToVector3() * 0.78f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Main.instance.LoadItem(_itemId);
        Asset<Texture2D> texture = TextureAssets.Item[_itemId];
        Rectangle frame = texture.Frame(1, 3);
        spriteBatch.Draw(texture.Value, NPC.Center - screenPos, frame, drawColor, (float)(Math.Sin(Main.timeForVisualEffects / 30.0) * 0.2), frame.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);
        return false;
    }

    public override void DrawEffects(ref Color drawColor)
    {
        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.PinkTorch, NPC.velocity.X, NPC.velocity.Y, 0, Color.White, 1f);
        dust.noGravity = true;
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        modifiers.Knockback *= 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        target.AddBuff(ModContent.BuffType<ForceFed>(), (int)(ForceFed.TicksPerCycle * 1.5f));
        target.AddBuff(BuffID.WellFed, 60 * 4);
        SoundEngine.PlaySound(SoundID.Item2, NPC.Center);
        NPC.life = 0;
    }
}
