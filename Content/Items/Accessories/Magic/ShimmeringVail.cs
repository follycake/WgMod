using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Magic;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class ShimmeringVail : ModItem
{
    WgStat _projectileCount = new(6f, 12f);
    WgStat _damage = new(80f, 120f);
    WgStat _invincibility = new(1f * 60f, 2f * 60f);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;

        Item.accessory = true;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.buyPrice(gold: 5);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out ShimmeringVailPlayer sv))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _projectileCount.Lerp(immobility);
        _damage.Lerp(immobility);
        _invincibility.Lerp(immobility);

        sv.ProjectileCount = _projectileCount;
        sv.Damage = _damage;
        sv.Invincibility = _invincibility;
        sv.Active = true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_projectileCount, _damage, _invincibility / 60);
    }
}

public class ShimmeringVailPlayer : ModPlayer
{
    const int CooldownMax = 5 * 60;

    public bool Active;

    public int ProjectileCount;
    public int Damage;
    public int Invincibility;
    int _cooldown = CooldownMax;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void PostUpdate()
    {
        if (!Active || Player != Main.LocalPlayer)
            return;

        if (_cooldown < CooldownMax)
            _cooldown++;

        if (_cooldown == CooldownMax - 1)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, Player.Center);

            for (int i = 0; i < ProjectileCount; i++)
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.HallowedTorch);
        }
    }

    public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
    {
        EverlastingRainbows(Player);
    }

    public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
    {
        EverlastingRainbows(Player);
    }

    public void EverlastingRainbows(Player player)
    {
        if (!Active || player != Main.LocalPlayer || _cooldown < CooldownMax)
            return;

        SoundEngine.PlaySound(SoundID.Item163, Player.Center);

        for (int i = 0; i < ProjectileCount; i++)
        {
            float angle = i / (float)ProjectileCount * MathF.Tau;
            Vector2 velocity = new(MathF.Cos(angle), MathF.Sin(angle));

            Dust.NewDust(Player.position, Player.width, Player.height, DustID.HallowedTorch);

            Projectile everlastingRainbows = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, velocity * 5f, ProjectileID.HallowBossLastingRainbow, (int)Player.GetTotalDamage(DamageClass.Magic).ApplyTo(Damage), 1f);

            everlastingRainbows.friendly = true;
            everlastingRainbows.hostile = false;
            everlastingRainbows.Size *= 0.5f;
        }

        _cooldown = 0;
    }

    public override void PostHurt(Player.HurtInfo info)
    {
        if (!Active || Player != Main.LocalPlayer || info.PvP)
            return;

        Player.AddImmuneTime(info.CooldownCounter, Invincibility);
    }
}
