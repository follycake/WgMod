using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class MeteorCrush : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 40;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (player.TryGetModPlayer(out MeteorCrushPlayer crush))
            crush._crushEffect = true;
    }
}

public class MeteorCrushPlayer : ModPlayer
{
    internal bool _crushEffect;

    /// <summary> If the player is on the ground or not.<br/>
    /// 0 = The player is in the air. <br/>
    /// 1 = The player just landed. <br/>
    /// 2 = The player is on the ground. <br/>
    /// </summary>
    internal int _landState = 2;

    float _yVelocityOfLastTick = 0;

    public override void ResetEffects()
    {
        _crushEffect = false;
    }

    public override void PostUpdate()
    {
        if (!_crushEffect)
            return;
        CheckForSolidGround();
        Projectile hitbox = null;
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.active && proj.type == ModContent.ProjectileType<Girth>() && proj.owner == Player.whoAmI)
            {
                hitbox = proj;
                break;
            }
        }
        if (hitbox == null)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
            else if (Player == Main.LocalPlayer)
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.position, Vector2.Zero, ModContent.ProjectileType<Girth>(), 0, 0, Player.whoAmI);
        }
        if (_landState == 1)
        {
            float crushPower = GetCrushPower();
            if (crushPower > 10)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                else if (Player == Main.LocalPlayer)
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Girthquake>(), (int)crushPower, 0, Player.whoAmI, 0, 8f + crushPower / 12f);
                SoundEngine.PlaySound(WgSounds.Stomp, Player.Center);
            }
        }
        _yVelocityOfLastTick = Player.velocity.Y;
    }

    float GetCrushPower()
    {
        return _yVelocityOfLastTick * Player.Wg().Weight.Mass / 120f - 10;
    }

    bool CheckForSolidGround()
    {
        List<Point> tiles = Collision.GetTilesIn(Player.Hitbox.BottomLeft() - new Vector2(-2, -2), Player.Hitbox.BottomRight() + new Vector2(2, 6));
        bool hasSolidTile = false;
        foreach (var point in tiles)
        {
            Tile tile = Framing.GetTileSafely(point);
            if (tile.HasTile)
            {
                if (Main.tileSolid[tile.TileType])
                    hasSolidTile = true;
                if (Main.tileSolidTop[tile.TileType])
                    hasSolidTile = true;
            }
        }
        if (hasSolidTile)
        {
            if (_landState == 0)
                _landState = 1;
            else
                _landState = 2;
        }
        else
            _landState = 0;
        return hasSolidTile;
    }

    public override void DrawPlayer(Camera camera)
    {
        if (_landState != 0)
            return;
        float crushPower = GetCrushPower();
        if (crushPower < 10f)
            return;
        float fade = Utils.Remap(crushPower, 10f, 30f, 0f, 1f);
        int totalShadows = Math.Min(Player.availableAdvancedShadowsCount, 10);
        int skip = 2;
        for (int i = totalShadows - totalShadows % skip; i > 0; i -= skip)
        {
            EntityShadowInfo shadowInfo = Player.GetAdvancedShadow(i);
            float shadow = Utils.Remap((float)i / totalShadows, 0f, 1f, 0.5f, 1f);
            shadow = float.Lerp(1f, shadow, fade);
            Main.PlayerRenderer.DrawPlayer(camera, Player, shadowInfo.Position + new Vector2(0f, Player.gfxOffY), shadowInfo.Rotation, shadowInfo.Origin, shadow);
        }
    }
}
