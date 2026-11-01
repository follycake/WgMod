using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items;

public abstract class RecoilItem : ModItem
{
    protected float _airTime = 1f;

    /// <summary>
    /// Recoil stats.
    /// </summary>
    /// <param name="recoilStrength"> The power of the applied recoil. </param>
    /// <param name="recoilResistance"> How much the player can resist the applied recoil, must be a clamped float. </param>
    /// <param name="airTimeFactor"> 
    /// How much firing in the air repeatedly reduces the recoil, must be a clamped float. Intended to prevent janky permaflight. 
    /// <para> Defaults to 1f </para>
    /// </param>
    /// <param name="flipRecoil"> 
    /// Makes the recoil send towards the player's mouse instead.  
    /// <para> Defaults to false. </para>
    /// </param>
    public abstract void ModifyRecoilStats(ref float recoilStrength, ref float airTimeFactor, ref bool flipRecoil);

    /// <summary>
    /// This creates recoil by applying velocity to the player against the direction of their mouse.
    /// </summary>
    /// <param name="recoilStrength"> The power of the applied recoil. </param>
    /// <param name="recoilResistance"> How much the player can resist the applied recoil, must be a clamped float. </param>
    /// <param name="airTimeFactor"> 
    /// How much firing in the air repeatedly reduces the recoil, must be a clamped float. Intended to prevent janky permaflight. 
    /// <para> Defaults to 1f </para>
    /// </param>
    /// <param name="flipRecoil"> 
    /// Makes the recoil send towards the player's mouse instead.  
    /// <para> Defaults to false. </para>
    /// </param>
    public void RecoilPlayer(Player player, float recoilStrength, float airTimeFactor = 1f, bool flipRecoil = false)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || player.noKnockback)
            return;

        Vector2 mousePosition = Main.MouseWorld;
        float angle = Utils.AngleFrom(player.Center, mousePosition);
        Vector2 velocity = new(MathF.Cos(angle), MathF.Sin(angle));

        if (!player.CheckForSolidGround())
        {
            recoilStrength *= _airTime;
            if (_airTime > airTimeFactor)
                _airTime -= airTimeFactor;
            else
                _airTime = airTimeFactor;
        }

        int direction = 1;
        if (flipRecoil)
            direction = -1;
        player.velocity += velocity * recoilStrength * (1f - wg.Weight.ClampedImmobility) * direction;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float recoilStrength = 0f;
        float airTimeFactor = 1f;
        bool flipRecoil = false;
        ModifyRecoilStats(ref recoilStrength, ref airTimeFactor, ref flipRecoil);
        RecoilPlayer(player, recoilStrength, airTimeFactor, flipRecoil);
        return true;
    }

    public override void UpdateInventory(Player player)
    {
        if (player.CheckForSolidGround())
            _airTime = 1;
    }
}
