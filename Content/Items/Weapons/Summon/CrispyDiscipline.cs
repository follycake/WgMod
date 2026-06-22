using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;
using WgMod.Content.Projectiles.Whip;

namespace WgMod.Content.Items.Weapons.Summon;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class CrispyDiscipline : ModItem
{
    WgStat _damage = new(1f, 1.4f);
    WgStat _knockback = new(1f, 2f);

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Caramelized.TagDamage);

    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 34;

        Item.DamageType = DamageClass.SummonMeleeSpeed;
        Item.damage = 25;
        Item.knockBack = 1.5f;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 1, silver: 50);

        Item.shoot = ModContent.ProjectileType<CrispyDisciplineProjectile>();
        Item.shootSpeed = 4;

        Item.useStyle = ItemUseStyleID.Swing;
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.UseSound = SoundID.Item152;
        Item.noMelee = true;
        Item.noUseGraphic = true;
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

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrispyHoneyBlock, 12)
            .AddIngredient(ItemID.BeeWax, 6)
            .AddIngredient(ItemID.Rope, 24)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override bool MeleePrefix()
    {
        return true;
    }
}
