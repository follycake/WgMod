using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using WgMod.Common.Systems;

namespace WgMod.Content.SkyEntities;

public class BigHarpy : ModSkyEntity
{
    public override void Spawn()
    {
        if (EventSystem.harpyMigration)
            Direction = -1;
        else
            Direction = Random.NextFloat() > 0.5f ? 1 : -1;

        VirtualCamera virtualCamera = new(Player);
        int offset = Random.Next(100, 1000);
        if (Direction < 0)
            Position.X = virtualCamera.Position.X + virtualCamera.Size.X + offset;
        else
            Position.X = virtualCamera.Position.X - offset;

        Position.Y = Random.NextFloat() * ((float)Main.worldSurface * 16f - 1600f - 2400f) + 2400f;
        Depth = Random.NextFloat() * 3f + 3f;
        SetPositionInWorldBasedOnScreenSpace(Position);

        Frame = new SpriteFrame(1, 4);
        FramingSpeed = 5;
        FrameOffset = Random.Next(0, FramingSpeed * Frame.RowCount); // Randomize starting frame

        LifeTime = Random.Next(40, 71) * 60;
        OpacityNormalizedTimeToFadeIn = 0.15f;
        OpacityNormalizedTimeToFadeOut = 0.85f;
        BrightnessLerper = 0.2f;
    }

    public override bool? ShouldSpawn()
    {
        if (!Main.dayTime)
            return false;
        if (EventSystem.harpyMigration)
            return true;
        return null;
    }

    public override float SpawnChance()
    {
        return 1f;
    }

    public override int SpawnCount()
    {
        if (EventSystem.harpyMigration)
            return 100;
        return Random.Next(1, 4);
    }

    public override void UpdateVelocity(int frameCount)
    {
        float speed = 3f + Math.Abs(Main.WindForVisuals) * 0.8f;
        Velocity = new Vector2(speed * Direction, -0.2f);
    }
}
