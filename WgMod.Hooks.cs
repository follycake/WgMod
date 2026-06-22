using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;
using WgMod.Content.Dusts;
using WgMod.Content.Tiles;

namespace WgMod;

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
        On_TileObject.CanPlace += TileObject_CanPlace;
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
        On_TileObject.CanPlace -= TileObject_CanPlace;
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
                    wg.SetWeight(wg.Weight + gain.TotalGain);
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
        float scale = 1f;
        if (drawPlayer.TryGetModPlayer(out WgPlayer wg) && self.Active)
        {
            int stage = wg.Weight.GetStage();
            Position.Y += WeightValues.DrawOffsetY(stage);
            scale = WeightValues.GetMountScale(stage);
        }
        int start = playerDrawData.Count;
        orig(self, playerDrawData, drawType, drawPlayer, Position, drawColor, playerEffect, shadow);
        if (scale > 1f)
        {
            Span<DrawData> span = CollectionsMarshal.AsSpan(playerDrawData);
            for (int i = start; i < playerDrawData.Count; i++)
            {
                ref DrawData data = ref span[i];
                data.scale *= scale;
            }
        }
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

    // For custom tombstones (temporary)
    static bool TileObject_CanPlace(On_TileObject.orig_CanPlace orig, int x, int y, int type, int style, int dir, out TileObject objectData, bool onlyCheck, int? forcedRandom, bool checkStay)
    {
        if (type == TileID.Tombstones && !Main.dedServ && Main.LocalPlayer.dead)
            FatTombstones.ReplaceTombstone(Main.LocalPlayer, ref type, ref style);
        return orig(x, y, type, style, dir, out objectData, onlyCheck, forcedRandom, checkStay);
    }
}
