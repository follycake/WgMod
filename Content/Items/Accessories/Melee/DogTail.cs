using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Projectiles.Melee;

namespace WgMod.Content.Items.Accessories.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class DogTail : GlobalItem
{
    WgStat _damageModifier = new(1f, 1.7f);

    public override bool InstancePerEntity => true;

    public override void SetDefaults(Item item)
    {
        if (item.type != ItemID.DogTail)
            return;

        item.defense = 7;
        item.vanity = false;
        item.rare = ItemRarityID.Cyan;
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (item.type != ItemID.DogTail || !player.TryGetModPlayer(out DogTailPlayer dt) || !player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        dt._active = true;

        _damageModifier.Lerp(immobility);
        dt._damageModifier = _damageModifier;

        if (dt._cooldown < 120)
            dt._cooldown++;

        if (Main.hardMode)
            dt._damage = 77;
        else
            dt._damage = 17;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (item.type != ItemID.DogTail)
            return;

        tooltips.LineBeforeTooltip(out TooltipLine line);
        tooltips.Insert(
            tooltips.IndexOf(line) + 1,
            new TooltipLine(
                Mod,
                "NewTooltip",
                Language.GetTextValue("Mods.WgMod.Items.DogTail.Tooltip")
            )
        );
    }
}

public class DogTailPlayer : ModPlayer
{
    public bool _active;
    public int _cooldown = 120;
    public int _damage;
    public float _damageModifier;

    public override void ResetEffects()
    {
        _active = false;
    }
}

public class DogTailItem : GlobalItem
{
    public override void UseAnimation(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out DogTailPlayer dt) || !dt._active || dt._cooldown < 120 || item.damage < 1 || player.whoAmI != Main.myPlayer)
            return;

        if (Main.hardMode)
            dt._cooldown = 60;
        else
            dt._cooldown = 0;

        if (player.wereWolf)
        {
            dt._cooldown += 30;
        }

        Vector2 mousePosition = Main.MouseWorld;
        float angle = Utils.AngleTo(player.Center, mousePosition);
        Vector2 velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

        {
            switch (item.useStyle)
            {
                case ItemUseStyleID.Swing:
                case ItemUseStyleID.Thrust:
                case ItemUseStyleID.Shoot:
                case ItemUseStyleID.Guitar:
                case ItemUseStyleID.Rapier:
                case ItemUseStyleID.RaiseLamp:
                    Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        player.Center,
                        velocity * 6f,
                        ModContent.ProjectileType<BouncyBall>(),
                        (int)(dt._damage * dt._damageModifier),
                        5
                    );
                    break;
            }
        }
    }
}
