using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Projectiles.Melee;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class HoneyedHamBat : ModItem
{
    WgStat _damage = new(1f, 2f);
    WgStat _knockback = new(1f, 2f);

    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.HamBat);

        Item.rare = ItemRarityID.Pink;
        Item.value = Item.buyPrice(gold: 10);

        Item.damage = (int)(Item.damage * 2f);
        Item.useTime = (int)(Item.useTime * 1.5f);
        Item.useAnimation = (int)(Item.useAnimation * 1.5f);
        Item.scale *= 1.5f;
    }

    public override void UpdateInventory(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        _damage.Lerp(immobility);
        _knockback.Lerp(immobility);
    }

    public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
    {
        damage *= _damage;
    }

    public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
    {
        knockback *= _knockback;
    }

    public override void MeleeEffects(Player player, Rectangle hitbox)
    { // Took inspiration from Calamity mod's "UltimusCleaver" item
        float angle;
        Vector2 velocity;
        Vector2 swordTipper = hitbox.Center.ToVector2();

        if (
            player.itemAnimation == (int)(player.itemAnimationMax * 0.25)
            || player.itemAnimation == (int)(player.itemAnimationMax * 0.5)
            || player.itemAnimation == player.itemAnimationMax
        )
        {
            angle = Utils.AngleTo(player.Center, swordTipper);
            velocity = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

            Projectile.NewProjectile(
                player.GetSource_FromThis(),
                player.Center,
                velocity * 10,
                ModContent.ProjectileType<HoneyGlob>(),
                (int)(Item.damage * 0.66666666),
                Item.knockBack * 0.25f
            );
        }
    }

    public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
    {
        player.AddBuff(BuffID.HeartyMeal, 7 * 60);
    }

    public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
    {
        player.AddBuff(BuffID.HeartyMeal, 7 * 60);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HamBat)
            .AddIngredient(ItemID.ChlorophyteBar, 12)
            .AddIngredient(ItemID.BottledHoney, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}
