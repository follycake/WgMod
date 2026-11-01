using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles.Minions;

namespace WgMod.Content.Items.Weapons.Summon;

public class ImpScepter : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
        ItemID.Sets.LockOnIgnoresCollision[Type] = true;

        ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
    }

    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.knockBack = 3f;
        Item.mana = 10;
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 36;
        Item.useAnimation = 36;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.value = Item.sellPrice(gold: 30);
        Item.rare = ItemRarityID.Cyan;
        Item.UseSound = SoundID.Item44;

        Item.noMelee = true;
        Item.DamageType = DamageClass.Summon;
        Item.buffType = ModContent.BuffType<IndulgentImpBuff>();
        Item.shoot = ModContent.ProjectileType<IndulgentImp>();
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = Main.MouseWorld;
        player.LimitPointToPlayerReachableArea(ref position);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!player.TryGetModPlayer(out IndulgentImpController ii))
            return false;

        if (!player.HasBuff(Item.buffType))
            player.AddBuff(Item.buffType, 2);

        if (ii.Stage < 3)
            ii.Stage++;
        else
            ii.QuickTeleport = true;

        return true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.ImpStaff)
            .AddIngredient(ItemID.SoulofNight, 6)
            .Register();
    }
}