using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod;

public static class Utility
{
    public static WgPlayer Wg(this Player player)
    {
        return player.GetModPlayer<WgPlayer>();
    }

    public static bool FormatLines(this List<TooltipLine> tooltips, params object[] args)
    {
        int start = tooltips.FindIndex(t => t.Name == "Tooltip0");
        if (start < 0)
            return false;
        for (int i = start; i < tooltips.Count; i++)
        {
            TooltipLine line = tooltips[i];
            if (!line.Name.StartsWith("Tooltip"))
                break;
            if (!line.Text.Contains('{'))
                continue;
            line.Text = string.Format(line.Text, args);
        }
        return true;
    }

    public static bool Format(this List<TooltipLine> tooltips, string name, params object[] args)
    {
        TooltipLine tooltip = tooltips.Find(t => t.Name == name);
        if (tooltip == null)
            return false;
        tooltip.Text = string.Format(tooltip.Text, args);
        return true;
    }

    public static string Red(this string value)
    {
        return $"[c/C42254:{value}]";
    }

    public static string Range(this int value, int min, int max)
    {
        return Range(value.ToString(), min.ToString(), max.ToString());
    }

    public static string Range(this string value, string min, string max)
    {
        if (value.Equals(min, StringComparison.InvariantCultureIgnoreCase))
            return OutOf(value.Red(), max);
        return OutOf(value, max);
    }

    public static string OutOf(this string value, string max)
    {
        return $"{value}[c/B000B0:/{max}]";
    }

    public static string Percent(this float value)
    {
        int rounded = (int)MathF.Round(value * 100f);
        if (rounded == 0)
            return rounded.ToString().Red();
        return rounded.ToString();
    }

    public static string Percent(this float value, float max)
    {
        return value.Percent().OutOf(max.Percent());
    }

    /// <summary>
    /// Attempts to get the TooltipLine of the list right before where the "tooltip" line should be.
    /// </summary>
    /// <param name="tooltips">The list of TooltipLine to look in.</param>
    /// <param name="line">The TooltipLine that would be directly above the "tooltip" line.</param>
    public static bool LineBeforeTooltip(this List<TooltipLine> tooltips, out TooltipLine line)
    {
        line = tooltips.Find(x => x.Name == "Material")
            ?? tooltips.Find(x => x.Name == "Consumable")
            ?? tooltips.Find(x => x.Name == "Ammo")
            ?? tooltips.Find(x => x.Name == "Placeable")
            ?? tooltips.Find(x => x.Name == "UseMana")
            ?? tooltips.Find(x => x.Name == "HealMana")
            ?? tooltips.Find(x => x.Name == "HealLife")
            ?? tooltips.Find(x => x.Name == "TileBoost")
            ?? tooltips.Find(x => x.Name == "HammerPower")
            ?? tooltips.Find(x => x.Name == "AxePower")
            ?? tooltips.Find(x => x.Name == "PickPower")
            ?? tooltips.Find(x => x.Name == "Defense")
            ?? tooltips.Find(x => x.Name == "Vanity")
            ?? tooltips.Find(x => x.Name == "Quest")
            ?? tooltips.Find(x => x.Name == "WandConsumes")
            ?? tooltips.Find(x => x.Name == "Equipable")
            ?? tooltips.Find(x => x.Name == "BaitPower")
            ?? tooltips.Find(x => x.Name == "NeedsBait")
            ?? tooltips.Find(x => x.Name == "FishingPower")
            ?? tooltips.Find(x => x.Name == "Knockback")
            ?? tooltips.Find(x => x.Name == "SpecialSpeedScaling")
            ?? tooltips.Find(x => x.Name == "NoSpeedScaling")
            ?? tooltips.Find(x => x.Name == "Speed")
            ?? tooltips.Find(x => x.Name == "CritChance")
            ?? tooltips.Find(x => x.Name == "Damage")
            ?? tooltips.Find(x => x.Name == "SocialDesc")
            ?? tooltips.Find(x => x.Name == "Social")
            ?? tooltips.Find(x => x.Name == "NoTransfer")
            ?? tooltips.Find(x => x.Name == "FavoriteDesc")
            ?? tooltips.Find(x => x.Name == "Favorite")
            ?? tooltips.Find(x => x.Name == "ItemName");
        return line != null;
    }

    public static int TimeToTicks(int hours = 0, int minutes = 0, int seconds = 0, int ticks = 0)
    {
        seconds *= 60;
        minutes *= 60 * 60;
        hours *= 60 * 60 * 60;
        return ticks + seconds + minutes + hours;
    }

    public static float GetJumpSpeedIncrease(float speed) => speed / 6.51f;

    public static ModPacket GetPacket(this Mod mod, WgMod.MessageType type, int capacity = 256)
    {
        ModPacket packet = mod.GetPacket(capacity);
        packet.Write((byte)type);
        return packet;
    }

    public static bool ShouldHidePlayer(this PlayerDrawSet drawInfo)
    {
        return drawInfo.drawPlayer.invis || drawInfo.drawPlayer.dead || drawInfo.hideEntirePlayer;
    }

    public static void TouchLava(this Player player)
    {
        if (player.lavaWet)
            return;
        if (player.lavaTime > 0)
            player.lavaTime--; // Account for the lavaTime increase when not touching lava
        if (!player.lavaImmune && player.whoAmI == Main.myPlayer && player.hurtCooldowns[4] <= 0)
        {
            if (player.lavaTime > 0)
                player.lavaTime--;
            else
            {
                int num87 = 80;
                int num88 = 420;
                if (Main.remixWorld)
                {
                    num87 = 200;
                    num88 = 630;
                }
                if (!player.ashWoodBonus || !player.lavaRose)
                {
                    if (player.ashWoodBonus)
                    {
                        if (Main.remixWorld)
                            num87 = 145;
                        num87 /= 2;
                        num88 -= 210;
                    }
                    if (player.lavaRose)
                    {
                        num87 -= 45;
                        num88 -= 210;
                    }
                    if (num87 > 0)
                        player.Hurt(PlayerDeathReason.ByOther(2), num87, 0, pvp: false, quiet: false, 4);
                    if (num88 > 0)
                        player.AddBuff(BuffID.OnFire, num88);
                }
            }
            player.lavaWet = true;
        }
    }
}
