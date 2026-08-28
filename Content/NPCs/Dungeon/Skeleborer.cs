using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;
using WgMod.Content.Items;
using WgMod.Content.Items.Placeable.Banners;

namespace WgMod.Content.NPCs.Dungeon;

public class SkeleborerHead : WormHead
{
    const int AttackCounterMax = 6 * 60;

    int _attackCounter;

    public override int BodyType => ModContent.NPCType<SkeleborerBody>();
    public override int TailType => ModContent.NPCType<SkeleborerTail>();

    // Server only
    WeightedRandom<int> _commonDrops;
    WeightedRandom<int> _rareDrops;

    public override void SetStaticDefaults()
    {
        var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = "WgMod/Content/NPCs/Dungeon/Skeleborer_Bestiary",
            Position = new Vector2(40f, 24f),
            PortraitPositionXOverride = 0f,
            PortraitPositionYOverride = 12f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }

    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerHead);

        NPC.aiStyle = -1;
        NPC.width = 58;
        NPC.height = 58;
        NPC.lifeMax = 2000;
        NPC.damage = 40;
        NPC.defense = 0;
        NPC.lifeRegen += 6;
        NPC.rarity = 4;
        NPC.value = 5 * 100 * 100;

        DrawOffsetY = 14;

        Banner = Type;

        BannerItem = ModContent.ItemType<SkeleborerBanner>();
        ItemID.Sets.KillsToBanner[BannerItem] = 25;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
                new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.Skeleborer")
        ]);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return SpawnCondition.DungeonNormal.Chance * 0.001f;
    }

    public override void Init()
    {
        MinSegmentLength = 36;
        MaxSegmentLength = 36;

        CommonWormInit(this);
    }

    internal static void CommonWormInit(Worm worm)
    {
        worm.MoveSpeed = 16f;
        worm.Acceleration = 0.1f;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(_attackCounter);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _attackCounter = reader.ReadInt32();
    }

    public override void OnSpawn(IEntitySource source) // Only called on the server
    {
        _commonDrops = new();
        _commonDrops.Add(ItemID.BlueBrick, 2);
        _commonDrops.Add(ItemID.PinkBrick, 2);
        _commonDrops.Add(ItemID.GreenBrick, 2);
        _commonDrops.Add(ItemID.Book, 1);
        if (NPC.downedPlantBoss)
            _commonDrops.Add(ItemID.Ectoplasm, 1);

        _rareDrops = new();
        _rareDrops.Add(ModContent.ItemType<SkeleborerIOU>(), 2);
        _rareDrops.Add(ItemID.WaterCandle, 1);
    }

    public override void AI()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            if (_attackCounter < AttackCounterMax)
                _attackCounter++;

            Player target = Main.player[NPC.target];
            if (_attackCounter >= AttackCounterMax && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
            {
                Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);

                SoundEngine.PlaySound(SoundID.Item8, NPC.position);

                for (int i = 0; i < 6; i++)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction.RotatedByRandom(MathHelper.ToRadians(90)) * 5, ProjectileID.LostSoulHostile, NPC.damage / 6, 0, Main.myPlayer);

                _attackCounter = 0;
                NPC.netUpdate = true;
            }
        }

        SkeleborerSystem.SkeleborerLighting(NPC);
    }

    public override void OnKill()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        Player target = Main.player[NPC.target];
        Vector2 position = NPC.Center;

        if (Collision.SolidCollision(NPC.Center, 0, 0))
            position = target.Center;

        Item.NewItem(NPC.GetSource_Death(), position, _rareDrops);

        for (int i = 0; i < Main.rand.Next(2, 5); i++)
            Item.NewItem(NPC.GetSource_Death(), position, _commonDrops, Main.rand.Next(10, 26));
    }
}

public class SkeleborerBody : WormBody
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawModifiers value = new()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        NPCID.Sets.RespawnEnemyID[Type] = ModContent.NPCType<SkeleborerHead>();
    }

    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerBody);

        NPC.aiStyle = -1;
        NPC.width = 58;
        NPC.height = 58;
        NPC.damage = 30;
        NPC.defense = 8;

        DrawOffsetY = 14;

        Banner = ModContent.NPCType<SkeleborerHead>();
    }

    public override void Init()
    {
        SkeleborerHead.CommonWormInit(this);
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        SkeleborerSystem.SkeleborerPierce(projectile, ref modifiers);
    }

    public override void AI()
    {
        SkeleborerSystem.SkeleborerLighting(NPC);
    }
}

public class SkeleborerTail : WormTail
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawModifiers value = new()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        NPCID.Sets.RespawnEnemyID[Type] = ModContent.NPCType<SkeleborerHead>();
    }

    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerTail);

        NPC.aiStyle = -1;
        NPC.width = 58;
        NPC.height = 58;
        NPC.damage = 20;
        NPC.defense = 10;

        DrawOffsetY = 14;

        Banner = ModContent.NPCType<SkeleborerHead>();
    }

    public override void Init()
    {
        SkeleborerHead.CommonWormInit(this);
    }

    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        SkeleborerSystem.SkeleborerPierce(projectile, ref modifiers);
    }

    public override void PostAI()
    {
        SkeleborerSystem.SkeleborerLighting(NPC);
    }
}

public class SkeleborerSystem : ModSystem
{
    internal static void SkeleborerLighting(NPC npc)
    {
        if (Collision.SolidCollision(npc.Center, 0, 0))
            return;

        Player target = Main.player[npc.target];

        float maxClamp = 500f;
        float distance = Math.Clamp(npc.position.Distance(target.position), 0f, maxClamp);

        if (distance == maxClamp)
            return;

        Vector3 light = new(140f, 238f, 255f);
        light /= 255f;

        light *= float.Lerp(1.5f, 0f, distance / maxClamp);

        Lighting.AddLight(npc.Center, light);
    }

    internal static void SkeleborerPierce(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.penetrate > 0)
            modifiers.FinalDamage *= 1f / projectile.maxPenetrate;
        else if (projectile.penetrate < 0)
            modifiers.FinalDamage *= 0.9f;
    }
}
