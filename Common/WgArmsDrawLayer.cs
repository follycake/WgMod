using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Common;

public class WgArmsDrawLayer : PlayerDrawLayer
{
    public override bool IsHeadLayer => false;
    public override Transformation Transform => PlayerDrawLayers.TorsoGroup;

    public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (!drawInfo.drawPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        int armStage = WeightValues.GetArmStage(stage);
        if (armStage < 0)
            return;

        Vector2 armPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);
        Vector2 vector2 = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
        vector2.Y -= 2f;
        armPosition += vector2 * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();

        float bodyRotation = drawInfo.drawPlayer.bodyRotation;
        float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeFrontArmRotation;
        Vector2 bodyVect = drawInfo.bodyVect;
        Vector2 compositeOffset_FrontArm = new(5 * drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), 0f);
        bodyVect += compositeOffset_FrontArm;
        armPosition += compositeOffset_FrontArm;

        Vector2 shoulderPosition = armPosition + drawInfo.frontShoulderOffset;
        if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
            armPosition -= new Vector2(drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());

        SpriteSet.Layer layer = SpriteSet.Current.ArmLayers[armStage];
        bool drawArmor = WgArmor.ShouldDraw(drawInfo) && layer.UVArmor;

        Asset<Texture2D> texture = layer.Texture;
        int frameX = drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width;
        int frameY = drawInfo.compFrontArmFrame.Y / drawInfo.compFrontArmFrame.Height;
        Rectangle frame = texture.Frame(9, 4, frameX, frameY);

        bodyVect -= drawInfo.compFrontArmFrame.Size() * 0.5f;
        bodyVect += frame.Size() * 0.5f;

        if (drawArmor && !drawInfo.compShoulderOverFrontArm)
            DrawCompShoulder(ref drawInfo, shoulderPosition, bodyRotation, bodyVect);

        DrawData drawData = new(texture.Value, armPosition, frame, drawInfo.colorBodySkin, rotation, bodyVect, 1f, drawInfo.playerEffect)
        {
            shader = drawInfo.skinDyePacked
        };
        drawInfo.DrawDataCache.Add(drawData);

        if (drawArmor)
        {
            WgArmor.Draw(wg, ref drawInfo, drawData, layer);
            if (drawInfo.compShoulderOverFrontArm)
                DrawCompShoulder(ref drawInfo, shoulderPosition, bodyRotation, bodyVect);
        }

        bool drawTop = frameY == 0 && (frameX == 2 || frameX == 3) || frameY == 1 && frameX == 2;
        if (drawTop)
            WgPlayerDrawLayer.Draw(ref drawInfo, true);
    }

    static void DrawCompShoulder(ref PlayerDrawSet drawInfo, Vector2 position, float bodyRotation, Vector2 bodyVect)
    {
        if (drawInfo.hideCompositeShoulders || drawInfo.drawPlayer.body <= 0)
            return;
        Texture2D tex = TextureAssets.ArmorBodyComposite[drawInfo.drawPlayer.body].Value;
        PlayerDrawLayers.DrawCompositeArmorPiece(ref drawInfo, CompositePlayerDrawContext.FrontShoulder, new DrawData(tex, position, drawInfo.compFrontShoulderFrame, drawInfo.colorArmorBody, bodyRotation, bodyVect, 1f, drawInfo.playerEffect)
        {
            shader = drawInfo.cBody
        });
    }
}
