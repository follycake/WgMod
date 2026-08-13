using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Pets;
using WgMod.Content.Projectiles.Pets;

namespace WgMod.Content.Items.Pets;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class GorgeistHeart : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToVanitypet(ModContent.ProjectileType<LilGorgie>(), ModContent.BuffType<GorgeistPet>()); // Vanilla has many useful methods like these, use them! It sets rarity and value as well, so we have to overwrite those after

		Item.width = 28;
		Item.height = 20;
		Item.rare = ItemRarityID.Master;
		Item.master = true;
		Item.value = Item.sellPrice(0, 5);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		player.AddBuff(Item.buffType, 2);

		return false;
	}
}
