using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;

namespace WgMod.Content.Projectiles;

public class Girthquake : ModProjectile
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.alpha = 255;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 120;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Default;
        Projectile.penetrate = -1;
    }

    public override bool CanHitPlayer(Player target)
    {
        return false;
    }

    public override void AI()
    {
        float num = Projectile.ai[1];
        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] > 9f)
        {
            Projectile.Kill();
            return;
        }

        Projectile.velocity = Vector2.Zero;
        Projectile.position = Projectile.Center;
        Projectile.Size = new Vector2(16f, 8f) * MathHelper.Lerp(5f, num, Utils.GetLerpValue(0f, 9f, Projectile.ai[0]));
        Projectile.Center = Projectile.position;
        Point point = Projectile.TopLeft.ToTileCoordinates();
        Point point2 = Projectile.BottomRight.ToTileCoordinates();
        int num3 = Projectile.width / 2;
        if ((int)Projectile.ai[0] % 3 == 0)
        {
            int num4 = (int)Projectile.ai[0] / 3;
            for (int i = point.X; i <= point2.X; i++)
            {
                for (int j = point.Y; j <= point2.Y; j++)
                {
                    if (Vector2.Distance(Projectile.Center, new Vector2(i * 16, j * 16)) <= num3)
                    {
                        Tile tileSafely = Framing.GetTileSafely(i, j);
                        if (tileSafely.HasTile && Main.tileSolid[tileSafely.TileType] &&
                            !Main.tileSolidTop[tileSafely.TileType] && !Main.tileFrameImportant[tileSafely.TileType])
                        {
                            Tile tileSafely2 = Framing.GetTileSafely(i, j - 1);
                            if (!tileSafely2.HasTile || !Main.tileSolid[tileSafely2.TileType] ||
                                Main.tileSolidTop[tileSafely2.TileType])
                            {
                                int num5 = WorldGen.KillTile_GetTileDustAmount(true, tileSafely, i, j);
                                for (int k = 0; k < num5; k++)
                                {
                                    Dust dust = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                                    dust.velocity.Y -= 3f + num4 * 1.5f;
                                    dust.velocity.Y *= Main.rand.NextFloat();
                                    dust.velocity.Y *= 0.75f;
                                    dust.scale += num4 * 0.03f;
                                }

                                if (num4 >= 2)
                                {
                                    for (int m = 0; m < num5 - 1; m++)
                                    {
                                        Dust dust5 = Main.dust[WorldGen.KillTile_MakeTileDust(i, j, tileSafely)];
                                        dust5.velocity.Y -= 1f + num4;
                                        dust5.velocity.Y *= Main.rand.NextFloat();
                                        dust5.velocity.Y *= 0.75f;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Projectile.damage = (int)Math.Ceiling(Projectile.damage * 0.75f);
    }
}

public class Girth : ModProjectile
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public override void SetDefaults()
    {
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.alpha = 255;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 99999;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Default;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
    }

    public override bool CanHitPlayer(Player target)
    {
        return false;
    }

    public override void AI()
    {
        Player owner = Main.player[Projectile.owner];
        if (!owner.active || owner.dead || !owner.GetModPlayer<MeteorCrushPlayer>()._crushEffect)
            Projectile.Kill();
        else
        {
            Projectile.width = owner.width + 4;
            Projectile.height = owner.height + 4;
            Projectile.position.X = owner.position.X - 2;
            Projectile.position.Y = owner.position.Y - 2;
            int dmg = Math.Max((int)(owner.velocity.Length() * owner.Wg().Weight.Mass / 120f) - 10, 0);
            Projectile.damage = dmg;
        }
    }
}
