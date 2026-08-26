using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using WgMod.Common.Systems;

namespace WgMod.Content.Clouds;

public class BigHarpy : AnimatedCloud, IUpdateCloud
{
    public override int FrameCount => 4;
    public override double FrameDuration => 5.0;

    static int GetDir(Cloud cloud) => cloud.spriteDir == SpriteEffects.None ? -1 : 1;
    static void SetDir(Cloud cloud, int dir) => cloud.spriteDir = dir == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

    public override float SpawnChance()
    {
        if (EventSystem.harpyMigration)
            return 10f;

        if (Main.dayTime)
            return 1f;

        return 0f;
    }

    public override void OnSpawn(Cloud cloud)
    {
        if (EventSystem.harpyMigration)
            SetDir(cloud, -1);
        else
            SetDir(cloud, Main.rand.NextBool() ? 1 : -1);
    }

    public bool PreUpdate(Cloud cloud)
    {
        cloud.position.X += (0.5f * GetDir(cloud) + Main.windSpeedCurrent * 0.5f) * (float)Main.dayRate;
        cloud.position.Y -= 0.2f * (float)Main.dayRate;
        return false;
    }

    public void PostUpdate(Cloud cloud)
    {

    }

    public override bool Draw(SpriteBatch spriteBatch, Cloud cloud, int cloudIndex, ref DrawData drawData)
    {
        drawData.scale *= 0.5f;
        return base.Draw(spriteBatch, cloud, cloudIndex, ref drawData);
    }
}
