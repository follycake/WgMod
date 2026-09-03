using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod;

public interface IUpdateCloud
{
    /// <summary> Return true to do vanilla cloud updating </summary>
    bool PreUpdate(Cloud cloud);
    void PostUpdate(Cloud cloud);
}

public partial class WgMod
{
    // Put general hooks here, specific hooks can be placed in their respective ModPlayer
    public static void RegisterHooks()
    {
        On_Player.AddBuff += Player_AddBuff;
        On_Player.DelBuff += Player_DelBuff;
        On_Player.UpdateSocialShadow += Player_UpdateSocialShadow;
        On_PlayerDrawSet.HeadOnlySetup += PlayerDrawSet_HeadOnlySetup;
        On_Mount.Draw += Mount_Draw;
        On_Main.GetPlayerArmPosition += Main_GetPlayerArmPosition;
        On_Main.DrawProj_DrawExtras += Main_DrawProj_DrawExtras;
        On_Cloud.Update += Cloud_Update;
        On_PlayerDrawLayers.DrawStarboardRainbowTrail += OnPlayerDrawLayers_DrawStarboardRainbowTrail;
        On_PlayerDrawLayers.DrawPlayer_03_PortableStool += OnPlayerDrawLayers_DrawPlayer_03_PortableStool;
        On_PlayerDrawLayers.DrawPlayer_09_Wings += OnPlayerDrawLayers_DrawPlayer_09_Wings;
    }

    // Always remember to unregister your hooks
    static void UnregisterHooks()
    {
        On_Player.AddBuff -= Player_AddBuff;
        On_Player.DelBuff -= Player_DelBuff;
        On_Player.UpdateSocialShadow -= Player_UpdateSocialShadow;
        On_PlayerDrawSet.HeadOnlySetup -= PlayerDrawSet_HeadOnlySetup;
        On_Mount.Draw -= Mount_Draw;
        On_Main.GetPlayerArmPosition -= Main_GetPlayerArmPosition;
        On_Main.DrawProj_DrawExtras -= Main_DrawProj_DrawExtras;
        On_Cloud.Update -= Cloud_Update;
        On_PlayerDrawLayers.DrawStarboardRainbowTrail -= OnPlayerDrawLayers_DrawStarboardRainbowTrail;
        On_PlayerDrawLayers.DrawPlayer_03_PortableStool -= OnPlayerDrawLayers_DrawPlayer_03_PortableStool;
        On_PlayerDrawLayers.DrawPlayer_09_Wings -= OnPlayerDrawLayers_DrawPlayer_09_Wings;
    }

    static void Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
    {
        if (!self.TryGetModPlayer(out WgPlayer wg))
        {
            orig(self, type, timeToAdd, quiet, foodHack);
            return;
        }

        int previousTime = int.MinValue;
        if (self.HasBuff(type))
            previousTime = self.buffTime[self.FindBuffIndex(type)];
        orig(self, type, timeToAdd, quiet, foodHack);
        if (!self.HasBuff(type))
            return;

        int index = self.FindBuffIndex(type);
        wg.BuffDuration[index] = timeToAdd;

        if (wg._ignoreWgBuffTimer > 0)
            return;

        if (_buffTable.TryGetValue(type, out GainOptions gain))
        {
            gain.TotalGain = wg.FoodAbsorption.ApplyTo(gain.TotalGain);
            if (gain.IsInstant)
            {
                if (previousTime < timeToAdd - 2) // Apply once (2 ticks of leeway)
                    wg.AddStomach(gain.TotalGain);
            }
            else if (!self.HasBuff<GainingBuff>())
                GainingBuff.AddBuff(wg, gain);
        }
    }

    static void Player_DelBuff(On_Player.orig_DelBuff orig, Player self, int index)
    {
        if (self.TryGetModPlayer(out WgPlayer wg))
        {
            wg.BuffDuration[index] = 0;
            int num = 0;
            for (int i = 0; i < wg.BuffDuration.Length - 1; i++)
            {
                if (wg.BuffDuration[i] != 0)
                {
                    if (num < i)
                    {
                        wg.BuffDuration[num] = wg.BuffDuration[i];
                        wg.BuffDuration[i] = 0;
                    }
                    num++;
                }
            }
        }
        orig(self, index);
    }

    static void Player_UpdateSocialShadow(On_Player.orig_UpdateSocialShadow orig, Player self)
    {
        if (!self.TryGetModPlayer(out WgPlayer wg))
        {
            orig(self);
            return;
        }
        float lastOffY = self.gfxOffY;
        self.gfxOffY += wg._addedGfxOffY;
        orig(self);
        self.gfxOffY = lastOffY;
    }

    static void PlayerDrawSet_HeadOnlySetup(On_PlayerDrawSet.orig_HeadOnlySetup orig, ref PlayerDrawSet self, Player drawPlayer2, List<DrawData> drawData, List<int> dust, List<int> gore, float X, float Y, float Alpha, float Scale)
    {
        orig(ref self, drawPlayer2, drawData, dust, gore, X, Y, Alpha, Scale);
        self.Position.X -= Math.Max((self.drawPlayer.width / 2) - 10, 0);
    }

    static Vector2 Main_GetPlayerArmPosition(On_Main.orig_GetPlayerArmPosition orig, Projectile proj)
    {
        Player player = Main.player[proj.owner];
        float gfx = player.gfxOffY;
        if (ProjectileID.Sets.IsAWhip[proj.type])
            gfx = 0f;

        Vector2 vector = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
        if (player.direction != 1)
            vector.X = player.bodyFrame.Width - vector.X;
        if (player.gravDir != 1f)
            vector.Y = player.bodyFrame.Height - vector.Y;
        vector -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - player.height) / 2f;
        Vector2 pos = player.MountedCenter - new Vector2(player.width, player.height) / 2f + vector + Vector2.UnitY * gfx;
        if (player.mount.Active && player.mount.Type == MountID.Wolf)
        {
            pos.Y -= player.mount.PlayerOffsetHitbox;
            pos += new Vector2(12 * player.direction, -12f);
        }
        return player.RotatedRelativePoint(pos, false, true);
    }

    static void Mount_Draw(On_Mount.orig_Draw orig, Mount self, List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
    {
        if (!drawPlayer.TryGetModPlayer(out WgPlayer wg) || !self.Active)
        {
            orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);
            return;
        }

        int stage = wg.Weight.GetStage();
        float scale = WeightValues.GetMountScale(stage);
        Position.Y += SpriteSet.GetStage(stage).OffsetY - wg._mountOffY;

        int start = playerDrawData.Count;
        orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);

        if (self.Type == MountID.Scutlix) // Temporary fix
        {
            wg._mountOffY = 0f;
            return;
        }

        if (scale > 1f)
        {
            float averageOffset = 0f;
            Span<DrawData> span = CollectionsMarshal.AsSpan(playerDrawData);
            for (int i = start; i < playerDrawData.Count; i++)
            {
                ref DrawData data = ref span[i];
                Rectangle rect = data.sourceRect ?? data.texture.Frame();
                float offset = -(rect.Height - data.origin.Y) * (scale - 1f);
                averageOffset += offset;
                data.position.Y += offset;
                data.scale *= scale;
            }
            averageOffset /= playerDrawData.Count - start;
            wg._mountOffY = averageOffset;
        }
        else
            wg._mountOffY = 0f;
    }

    static void Main_DrawProj_DrawExtras(On_Main.orig_DrawProj_DrawExtras orig, Main self, Projectile proj, Vector2 mountedCenter, ref float polePosX, ref float polePosY)
    {
        Player plr = Main.player[proj.owner];
        if (plr.whoAmI >= 0 && plr.whoAmI < 255)
        {
            if (proj.aiStyle == ProjAIStyleID.Yoyo || proj.aiStyle == ProjAIStyleID.Drill)
                proj.gfxOffY = 0f;
        }
        orig(self, proj, mountedCenter, ref polePosX, ref polePosY);
    }

    static void Cloud_Update(On_Cloud.orig_Update orig, Cloud self)
    {
        if (self.ModCloud is IUpdateCloud update)
        {
            if (update.PreUpdate(self))
                orig(self);
            else
            {
                if (Main.bgAlphaFrontLayer[4] == 1f && self.position.Y > 200f)
                {
                    self.kill = true;
                    self.Alpha -= 0.005f * (float)Main.dayRate;
                }
                if (!self.kill)
                {
                    if (self.Alpha < 1f)
                    {
                        self.Alpha += 0.001f * (float)Main.dayRate;
                        if (self.Alpha > 1f)
                            self.Alpha = 1f;
                    }
                }
                else
                {
                    self.Alpha -= 0.001f * (float)Main.dayRate;
                    if (self.Alpha <= 0f)
                        self.active = false;
                }
                if (self.position.X + TextureAssets.Cloud[self.type].Width() * self.scale < 0f - 600f || self.position.X > Main.screenWidth + 600f)
                    self.active = false;
                self.width = (int)(TextureAssets.Cloud[self.type].Width() * self.scale);
                self.height = (int)(TextureAssets.Cloud[self.type].Height() * self.scale);
            }
            update.PostUpdate(self);
        }
        else
            orig(self);
    }
    public static void OnPlayerDrawLayers_DrawStarboardRainbowTrail(On_PlayerDrawLayers.orig_DrawStarboardRainbowTrail orig, ref PlayerDrawSet drawinfo, Vector2 commonWingPosPreFloor, Vector2 dirsVec)
    {
        if (drawinfo.shadow != 0f)
        {
            return;
        }
        int num = Math.Min(drawinfo.drawPlayer.availableAdvancedShadowsCount - 1, 30);
        float num2 = 0f;
        for (int num3 = num; num3 > 0; num3--)
        {
            EntityShadowInfo advancedShadow = drawinfo.drawPlayer.GetAdvancedShadow(num3);
            float num10 = num2;
            Vector2 position = drawinfo.drawPlayer.GetAdvancedShadow(num3 - 1).Position;
            num2 = num10 + Vector2.Distance(advancedShadow.Position, position);
        }
        float num4 = MathHelper.Clamp(num2 / 160f, 0f, 1f);
        Main.instance.LoadProjectile(250);
        Texture2D value = TextureAssets.Projectile[250].Value;
        float x = 1.7f;
        Vector2 origin = new Vector2((float)(value.Width / 2), (float)(value.Height / 2));
        Color white = Color.White;
        white.A = 64;
        Vector2 vector2 = new Vector2(drawinfo.drawPlayer.width, drawinfo.drawPlayer.height) * new Vector2(0.5f, 1f) + new Vector2(0f, -4f);
        if (dirsVec.Y < 0f)
        {
            vector2 = new Vector2(drawinfo.drawPlayer.width, drawinfo.drawPlayer.height) * new Vector2(0.5f, 0f) + new Vector2(0f, 4f);
        }
        for (int num5 = num; num5 > 0; num5--)
        {
            EntityShadowInfo advancedShadow2 = drawinfo.drawPlayer.GetAdvancedShadow(num5);
            EntityShadowInfo advancedShadow3 = drawinfo.drawPlayer.GetAdvancedShadow(num5 - 1);
            Vector2 pos = advancedShadow2.Position + vector2 + advancedShadow2.HeadgearOffset;
            Vector2 pos2 = advancedShadow3.Position + vector2 + advancedShadow3.HeadgearOffset;
            pos = drawinfo.drawPlayer.RotatedRelativePoint(pos, true, false);
            pos2 = drawinfo.drawPlayer.RotatedRelativePoint(pos2, true, false);
            float num6 = (pos2 - pos).ToRotation() - 1.5707964f;
            num6 = 1.5707964f * (float)drawinfo.drawPlayer.direction;
            float num7 = Math.Abs(pos2.X - pos.X);
            Vector2 scale = new Vector2(x, num7 / (float)value.Height);
            float num8 = 1f - (float)num5 / (float)num;
            num8 *= num8;
            num8 *= Terraria.Utils.GetLerpValue(0f, 4f, num7, true);
            num8 *= 0.5f;
            num8 *= num8;
            Color color = white * num8 * num4;
            if (!(color == Color.Transparent))
            {
                DrawData item = new DrawData(value, pos - Main.screenPosition, null, color, num6, origin, scale, drawinfo.playerEffect, 0f);
                item.shader = drawinfo.cWings;
                drawinfo.DrawDataCache.Add(item);
                for (float num9 = 0.25f; num9 < 1f; num9 += 0.25f)
                {
                    item = new DrawData(value, Vector2.Lerp(pos, pos2, num9) - Main.screenPosition, null, color, num6, origin, scale, drawinfo.playerEffect, 0f);
                    item.shader = drawinfo.cWings;
                    drawinfo.DrawDataCache.Add(item);
                }
            }
        }
    }
    public static void OnPlayerDrawLayers_DrawPlayer_03_PortableStool(On_PlayerDrawLayers.orig_DrawPlayer_03_PortableStool orig, ref PlayerDrawSet drawinfo)
    {
        if (drawinfo.drawPlayer.portableStoolInfo.IsInUse)
        {
            Texture2D value = TextureAssets.Extra[ExtrasID.PortableStool].Value;
            Vector2 position = new Vector2((float)((int)(drawinfo.Position.X - Main.screenPosition.X + (float)(drawinfo.drawPlayer.width / 2))), (float)((int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height + 28f - drawinfo.drawPlayer.gfxOffY)));
            Rectangle rectangle = value.Frame(1, 1, 0, 0, 0, 0);
            Vector2 origin = rectangle.Size() * new Vector2(0.5f, 1f);
            DrawData item = new DrawData(value, position, new Rectangle?(rectangle), drawinfo.colorArmorLegs, drawinfo.drawPlayer.bodyRotation, origin, 1f, drawinfo.playerEffect, 0f);
            item.shader = drawinfo.cPortableStool;
            drawinfo.DrawDataCache.Add(item);
        }
    }
    public static void OnPlayerDrawLayers_DrawPlayer_09_Wings(On_PlayerDrawLayers.orig_DrawPlayer_09_Wings orig, ref PlayerDrawSet drawinfo)
    {
        if (drawinfo.drawPlayer.dead || drawinfo.hideEntirePlayer || drawinfo.drawPlayer.wings <= 0)
        {
            return;
        }
        if (drawinfo.drawPlayer.wings != 45)
        {
            orig(ref drawinfo);
            return;
        }
        Vector2 directions = drawinfo.drawPlayer.Directions;
        Vector2 vector = drawinfo.Position - Main.screenPosition + drawinfo.drawPlayer.Size / 2f;
        Vector2 vector2 = new(0f, 7f);
        vector = drawinfo.Position - Main.screenPosition + new Vector2((float)(drawinfo.drawPlayer.width / 2), (float)(drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height / 2) - drawinfo.drawPlayer.gfxOffY) + vector2;
        Main.instance.LoadWings(drawinfo.drawPlayer.wings);
        if (!drawinfo.drawPlayer.ShouldDrawWingsThatAreAlwaysAnimated())
        {
            return;
        }
        PlayerDrawLayers.DrawStarboardRainbowTrail(ref drawinfo, vector, directions);
        Color color10 = new Color(255, 255, 255, 255);
        int num3 = 22;
        int num4 = 0;
        Vector2 vec2 = vector + new Vector2((float)num4, (float)num3) * directions;
        Color color2 = color10 * (1f - drawinfo.shadow);
        DrawData item = new DrawData(TextureAssets.Wings[drawinfo.drawPlayer.wings].Value, vec2.Floor(), new Rectangle?(new Rectangle(0, TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 6 * drawinfo.drawPlayer.wingFrame, TextureAssets.Wings[drawinfo.drawPlayer.wings].Width(), TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 6)), color2, drawinfo.drawPlayer.bodyRotation, new Vector2((float)(TextureAssets.Wings[drawinfo.drawPlayer.wings].Width() / 2), (float)(TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 12)), 1f, drawinfo.playerEffect, 0f);
        item.shader = drawinfo.cWings;
        drawinfo.DrawDataCache.Add(item);
        if (drawinfo.shadow == 0f)
        {
            float num5 = ((float)drawinfo.drawPlayer.miscCounter / 75f * 6.2831855f).ToRotationVector2().X * 4f;
            Color color3 = new Color(70, 70, 70, 0) * (num5 / 8f + 0.5f) * 0.4f;
            for (float num6 = 0f; num6 < 6.2831855f; num6 += 1.5707964f)
            {
                item = new DrawData(TextureAssets.Wings[drawinfo.drawPlayer.wings].Value, vec2.Floor() + num6.ToRotationVector2() * num5, new Rectangle?(new Rectangle(0, TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 6 * drawinfo.drawPlayer.wingFrame, TextureAssets.Wings[drawinfo.drawPlayer.wings].Width(), TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 6)), color3, drawinfo.drawPlayer.bodyRotation, new Vector2((float)(TextureAssets.Wings[drawinfo.drawPlayer.wings].Width() / 2), (float)(TextureAssets.Wings[drawinfo.drawPlayer.wings].Height() / 12)), 1f, drawinfo.playerEffect, 0f);
                item.shader = drawinfo.cWings;
                drawinfo.DrawDataCache.Add(item);
            }
        }
    }
}
