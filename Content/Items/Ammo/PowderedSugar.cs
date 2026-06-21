using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Items.Ammo;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class PowderedSugar : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.PurificationPowder);

        Item.shoot = ModContent.ProjectileType<PowderedSugarProjectile>();
        Item.ResearchUnlockCount = 99;
    }

    public override void UseAnimation(Player player)
    {
        Vector2 mousePosition = Main.MouseWorld;
        float angle = Utils.AngleTo(player.Center, mousePosition);
        Vector2 velocity = new(MathF.Cos(angle), MathF.Sin(angle));

        for (int i = 0; i < 100; i++)
        {
            Dust sugar = Dust.NewDustDirect(
                player.position,
                player.width,
                player.height,
                DustID.RainbowTorch,
                velocity.X * 6f,
                velocity.Y * 6f,
                Main.rand.Next(25, 75),
                default,
                Main.rand.NextFloat(0, 1)
            );
            sugar.noGravity = true;
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe(5)
            .AddIngredient(ItemID.BambooBlock, 1)
            .AddTile(TileID.Bottles).Register();
    }
}
