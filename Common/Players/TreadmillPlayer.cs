using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Content.Tiles;

namespace WgMod.Common.Players;

public class TreadmillPlayer : ModPlayer
{
    internal bool _onTreadmill;
    internal float _treadmillX;

    public override void Load()
    {
        On_PlayerSittingHelper.UpdateSitting += OnUpdateSitting;
    }

    public override void Unload()
    {
        On_PlayerSittingHelper.UpdateSitting -= OnUpdateSitting;
    }

    public override void PostUpdateBuffs()
    {
        if (_onTreadmill && Player.TryGetModPlayer(out WgPlayer wg))
            wg.MovementWeightLossRate += Treadmill.WeightLoss;
    }

    public override void PreUpdate()
    {
        if (!Player.sitting.isSitting)
            _onTreadmill = false;
    }

    public override void PostUpdate()
    {
        if (_onTreadmill)
            Player.Center = new Vector2(_treadmillX, Player.Center.Y);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (!_onTreadmill)
            return;
        drawInfo.isSitting = false;
        drawInfo.torsoOffset = 0f;
        drawInfo.seatYOffset = 0f;
    }

    static void OnUpdateSitting(On_PlayerSittingHelper.orig_UpdateSitting orig, ref PlayerSittingHelper self, Player player)
    {
        if (!player.TryGetModPlayer(out TreadmillPlayer tp) || !tp._onTreadmill)
        {
            orig(ref self, player);
            return;
        }
        bool left = player.controlLeft;
        bool right = player.controlRight;
        player.controlLeft = false;
        player.controlRight = false;
        orig(ref self, player);
        player.controlLeft = left;
        player.controlRight = right;
    }
}
