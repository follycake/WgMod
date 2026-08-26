using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Systems;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class Tailwinds : ModItem
{
    WgStat _modifier = new(1.15f, 1.5f);

    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 64;

        Item.expert = true;
        Item.accessory = true;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out TailwindsPlayer tp))
            return;

        _modifier.Lerp(WindSystem.clampedWind);

        tp.modifier = _modifier;

        tp.active = true;
        tp.hidden = hideVisual;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (!Item.TryGetGlobalItem(out TailwindsItem ti))
            return;

        tooltips.FormatLines((_modifier - 1).Percent());
    }
}

public class TailwindsPlayer : ModPlayer
{
    public bool active;
    public bool hidden;

    public float modifier;

    public override void ResetEffects()
    {
        active = false;
        hidden = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!active || Player != Main.LocalPlayer || hidden)
            return;

        if (WindSystem.windDirection == Player.direction && Main.rand.NextBool(60))
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.Cloud, Player.velocity.X + Main.windSpeedCurrent, Player.velocity.Y, 50);
    }
}

public class TailwindsItem : GlobalItem
{
    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (!player.TryGetModPlayer(out TailwindsPlayer tp) || !tp.active || player != Main.LocalPlayer)
            return;

        int particles = (int)((tp.modifier - 1f) * 10f);
        int projectileDirection;

        if (velocity.X > 0f)
            projectileDirection = 1;
        else
            projectileDirection = -1;

        if (projectileDirection == WindSystem.windDirection)
        {
            damage = (int)(damage * tp.modifier);
            velocity *= tp.modifier;

            for (int i = 0; i < particles; i++)
                Dust.NewDustDirect(position, 0, 0, DustID.Sand, velocity.X, velocity.Y, 50);
        }
    }
}
