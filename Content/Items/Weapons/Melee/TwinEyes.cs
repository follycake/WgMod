using WgMod.Content.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Weapons.Melee;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class TwinEyes : ModItem
{
	WgStat _damage = new(1f, 1.25f);
	WgStat _knockback = new(1f, 1.25f);

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
	}

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = 45;
		Item.useTime = 45;
		Item.knockBack = 7.5f;
		Item.width = 42;
		Item.height = 44;
		Item.damage = 64;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<TwinEyesRet>();
		Item.shootSpeed = 12f;
		Item.UseSound = SoundID.Item1;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.sellPrice(gold: 5, silver: 60);
		Item.DamageType = DamageClass.MeleeNoSpeed;
		Item.channel = true;
		Item.noMelee = true;
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
			.AddIngredient(ItemID.HallowedBar, 12)
			.AddIngredient(ItemID.SoulofSight, 20)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}
