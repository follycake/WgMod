using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgHeadDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => true;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Head, PlayerDrawLayers.ArmOverItem);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    public static Vector2 GetPosition(in PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        return new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.headPosition + drawInfo.headVect;
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.ShouldHidePlayer())
            return;
        Player player = drawInfo.drawPlayer;
        if (!player.TryGetModPlayer(out WgPlayer wg) || wg._headOverride != null)
            return;
        Vector2 position = GetPosition(drawInfo);
        int animFrame = player.bodyFrame.Y / player.bodyFrame.Height;
        if (animFrame >= 7 && animFrame <= 9 || animFrame >= 14 && animFrame <= 16)
            position.Y -= 2f;
        SpriteSet.Stage stageData = SpriteSet.GetStage(wg.Weight.GetStage(), out SpriteSet set);
        Color skinColor = WgPlayerDrawLayer.GetSkinColor(drawInfo);
        foreach (SpriteSet.Layer layer in set.HeadLayers)
        {
            Rectangle frame = layer.Frame(set, stageData);
            DrawData drawData = new(
                layer.Texture.Value,
                position,
                frame,
                skinColor,
                player.headRotation,
                drawInfo.headVect,
                1f,
                drawInfo.playerEffect
            )
            {
                shader = drawInfo.skinDyePacked
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}

public class WgBelowHeadDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => true;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (drawInfo.ShouldHidePlayer())
            return;
        Player player = drawInfo.drawPlayer;
        if (!player.TryGetModPlayer(out WgPlayer wg) || wg._headOverride == null)
            return;
        Vector2 position = WgHeadDrawLayer.GetPosition(drawInfo);
        DrawData drawData = new(
            wg._headOverride.Value,
            position,
            player.bodyFrame,
            player.GetImmuneAlpha(Color.White, drawInfo.shadow),
            player.headRotation,
            drawInfo.headVect,
            1f,
            drawInfo.playerEffect
        )
        {
            shader = drawInfo.cHead
        };
        drawInfo.DrawDataCache.Add(drawData);
    }
}
