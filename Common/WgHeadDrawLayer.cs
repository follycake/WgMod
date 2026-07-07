using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgHeadDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => true;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        if (player.invis || player.dead)
            return;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (wg._headOverride == null)
            return;
        DrawData head = new(wg._headOverride.Value, new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect, drawInfo.drawPlayer.bodyFrame, Color.White, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
        drawInfo.DrawDataCache.Add(head);
    }
}
