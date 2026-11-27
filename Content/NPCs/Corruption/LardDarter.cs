using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Common.Players;
using WgMod.Common.Systems;
using WgMod.Content.Items.Curses;
using WgMod.Content.Items.Placeable.Banners;

namespace WgMod.Content.NPCs.Corruption;
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class LardDarter_Corrupt : ModNPC
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailingMode[NPC.type] = 3;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;

        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Position = new Vector2(0, 4),
            PortraitPositionYOverride = 0f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    Vector2 _targetPosition = Vector2.Zero;
    float _finalRotation = 0f;
    bool _enraged = false;
    int _enragedCounter = 0;
    float _redness = 0f;

    public override void SetDefaults()
    {
        NPC.width = 32;
        NPC.height = 32;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;
        NPC.value = 280;
        NPC.lifeMax = 75;
        NPC.npcSlots = 1.5f;
        NPC.damage = 26;
        NPC.defense = 3;


        if (NPC.downedPlantBoss && Main.expertMode)
        {
            NPC.value = 520;
            NPC.lifeMax = 220;
            NPC.damage = 72;
            NPC.defense = 28;
        }
        else if (Main.hardMode && Main.expertMode)
        {
            NPC.value = 520;
            NPC.lifeMax = 190;
            NPC.damage = 60;
            NPC.defense = 26;
        }

        NPC.HitSound = SoundID.NPCHit13;
        NPC.DeathSound = SoundID.NPCDeath21;

        Banner = Type;
        BannerItem = ModContent.ItemType<LardDarterBanner>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.LardDarter")
        ]);
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        float chance = SpawnCondition.Corruption.Chance * 0.5f;
        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;
        return chance;
    }

    public float Easing(float t) => (float)-(Math.Cos(Math.PI * t) - 1) / 2;

    // ai0 = timer, ai1 = state
    // 0 = idle, 1 = aiming, 2 = charging, 3 = returning to idle
    public override void AI()
    {
        if (NPC.ai[1] == 2)
            NPC.velocity *= 0.96f;
        else
            NPC.velocity *= 0.92f;

        bool pong = false;

        if (NPC.collideX)
        {
            pong = true;
            NPC.velocity.X = NPC.oldVelocity.X * -1;
        }
        if (NPC.collideY)
        {
            pong = true;
            NPC.velocity.Y = NPC.oldVelocity.Y * -1;
        }

        if (pong)
            SoundEngine.PlaySound(SoundID.Dig, NPC.Center);

        if (_enraged)
            _redness = Math.Clamp(_redness + 0.08f, 0, 1);
        else
            _redness = Math.Clamp(_redness - 0.08f, 0, 1);

        NPC.TargetClosest(false);
        if (!NPC.HasValidTarget)
        {
            NPC.ai[1] = 0;
            return;
        }

        NPC.ai[0]++;

        switch (NPC.ai[1])
        {
            case 0:
                if (NPC.ai[0] >= 20)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 1;
                    _targetPosition = Main.player[NPC.target].Center;
                    NPC.netUpdate = true;
                }
                break;
            case 1:
                if (_enraged)
                {
                    if (NPC.ai[0] >= 10)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 2;
                        NPC.velocity = new Vector2(36, 0).RotatedBy(NPC.Center.AngleTo(_targetPosition));
                        SoundEngine.PlaySound(SoundID.Item42.WithPitchOffset(1.2f), NPC.Center);
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    float target = NPC.Center.AngleTo(_targetPosition) + MathHelper.PiOver2;
                    if (target > Math.PI)
                        target = (float)Math.PI * -2 + target;

                    NPC.rotation = float.Lerp(0f, target, Easing(Math.Min(NPC.ai[0] / 30f, 1)));
                    if (NPC.ai[0] >= 35)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 2;
                        if (Main.hardMode)
                            NPC.velocity = new Vector2(32, 0).RotatedBy(NPC.Center.AngleTo(_targetPosition));
                        else
                            NPC.velocity = new Vector2(28, 0).RotatedBy(NPC.Center.AngleTo(_targetPosition));
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        NPC.netUpdate = true;
                    }
                }
                break;
            case 2:
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                if (_enraged)
                {
                    if (NPC.ai[0] >= 30)
                    {
                        NPC.ai[0] = 0;
                        float distance = NPC.Distance(Main.player[NPC.target].Center);

                        _enragedCounter++;

                        if (distance > 600)
                        {
                            _enragedCounter = 0;
                        }
                        if (_enragedCounter >= 3)
                        {
                            _enraged = false;
                            NPC.ai[1] = 3;
                            _finalRotation = NPC.rotation;
                            _targetPosition = Main.player[NPC.target].Center;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.ai[1] = 1;
                            _targetPosition = Main.player[NPC.target].Center;
                            NPC.netUpdate = true;
                        }
                    }
                }
                else
                {
                    if (NPC.ai[0] >= 70)
                    {
                        NPC.ai[0] = 0;
                        float distance = NPC.Distance(Main.player[NPC.target].Center);
                        if (distance > 800 && Main.hardMode)
                        {
                            _enraged = true;
                            _enragedCounter = 0;
                            NPC.ai[1] = 1;
                            _targetPosition = Main.player[NPC.target].Center;
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.ai[1] = 3;
                            _finalRotation = NPC.rotation;
                            NPC.netUpdate = true;
                        }
                    }
                }
                break;
            case 3:
                float start2 = _finalRotation;
                if (start2 > Math.PI)
                    start2 = (float)Math.PI * -2 + start2;

                NPC.rotation = float.Lerp(start2, 0f, Easing(Math.Min(NPC.ai[0] / 60f, 1)));
                if (NPC.ai[0] >= 60)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;
                }
                break;
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(_targetPosition);
        writer.Write(_enraged);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _targetPosition = reader.ReadVector2();
        _enraged = reader.ReadBoolean();
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;

        if (NPC.frameCounter > 8 * 7)
            NPC.frameCounter = 0;

        int currentFrame = (int)Math.Floor(NPC.frameCounter / 8);

        if (NPC.ai[1] == 2)
        {
            NPC.frame = new Rectangle(496, 0, 62, 74);
            return;
        }

        NPC.frame = new Rectangle(62 * currentFrame, 0, 62, 74);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 origin = NPC.frame.Size() / 2f;

        drawColor = drawColor.MultiplyRGBA(new Color(1f, 1f - (_redness * 0.5f), 1f - (_redness * 0.5f), 1f));

        if (NPC.IsABestiaryIconDummy)
        {
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center, NPC.frame, drawColor, 0f, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }

        for (int k = NPC.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = NPC.oldPos[k] + NPC.Hitbox.Size() / 2 - Main.screenPosition;
            Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.oldRot[k], origin, 1f, SpriteEffects.None, 0);
        }

        spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, origin, 1f, SpriteEffects.None, 0f);

        return false;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        int counter = 0;

        if (NPC.life > 0)
        {
            int loops = 0;
            while ((double)loops < hit.Damage / (double)NPC.lifeMax * 100.0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs, (float)hit.HitDirection, -1f, NPC.alpha, NPC.color, NPC.scale);
                counter = loops;
                loops = counter + 1;
            }
            DumbParticleSystem.LardExplode(NPC.GetSource_FromThis(), (int)(hit.Damage / (double)NPC.lifeMax * 20.0), NPC.Center, 2f, false, true);
            return;
        }
        for (int num583 = 0; num583 < 50; num583 = counter + 1)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CorruptGibs, (float)hit.HitDirection, -2f, NPC.alpha, NPC.color, NPC.scale);
            counter = num583;
        }

        IEntitySource source = NPC.GetSource_Death();

        int segment = Mod.Find<ModGore>("LardDarter_Corrupt").Type;

        Gore.NewGore(source, NPC.Center, NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f)), segment, 1f);
        Gore.NewGore(source, NPC.Center, NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f)), segment, 1f);

        DumbParticleSystem.LardExplode(source, 40, NPC.Center, 4f, true, true);
        return;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        if (!target.TryGetModPlayer(out WgPlayer wg))
            return;
        Mass weightGain = 4f + hurtInfo.Damage / 15f;
        weightGain = wg.AddWeight(weightGain);
        SoundEngine.PlaySound(WgSounds.Gulp, target.Center);
        if (weightGain > 0f)
            wg.CombatWeightText(weightGain, true);
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        if (NPC.ai[1] != 2)
            modifiers.SourceDamage /= 2;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 1));
        npcLoot.Add(ItemDropRule.Common(ItemID.Burger, 100));
    }
}

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class LardDarter_Crimson : LardDarter_Corrupt
{
    public override string Texture => "WgMod/Content/NPCs/Crimson/LardDarter_Crimson";

    public override void SetDefaults()
    {
        base.SetDefaults();

        Banner = ModContent.NPCType<LardDarter_Corrupt>();
        BannerItem = ModContent.ItemType<LardDarterBanner>();
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.WgMod.Bestiary.LardDarter"),
        ]);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        float chance = SpawnCondition.Crimson.Chance * 0.5f;
        if (spawnInfo.Player.Curses().ActiveCurses[(int)CurseIDs.MoreFatteningEnemies])
            chance *= 2f;
        return chance;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        int counter = 0;

        if (NPC.life > 0)
        {
            int loops = 0;
            while ((double)loops < hit.Damage / (double)NPC.lifeMax * 100.0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, (float)hit.HitDirection, -1f, NPC.alpha, Scale: NPC.scale);
                counter = loops;
                loops = counter + 1;
            }
            DumbParticleSystem.LardExplode(NPC.GetSource_FromThis(), (int)(hit.Damage / (double)NPC.lifeMax * 20.0), NPC.Center, 2f, false, true);
            return;
        }
        for (int i = 0; i < 50; i = counter + 1)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, (float)hit.HitDirection, -2f, NPC.alpha, Scale: NPC.scale);
            counter = i;
        }

        IEntitySource source = NPC.GetSource_Death();

        int segment = Mod.Find<ModGore>("LardDarter_Crimson").Type;

        Gore.NewGore(source, NPC.Center, NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f)), segment, 1f);
        Gore.NewGore(source, NPC.Center, NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f)), segment, 1f);

        DumbParticleSystem.LardExplode(source, 40, NPC.Center, 4f, true, true);
        return;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 1));
        npcLoot.Add(ItemDropRule.Common(ItemID.Burger, 100));
    }
}
