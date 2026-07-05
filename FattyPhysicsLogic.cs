using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod;

public static class FattyPhysicsLogic
{
    public static float CompareSizeTo(this Entity entity1, Entity entity2)
    {
        return entity1.Size.Length() / entity2.Size.Length();
    }

    public static void ActuallyPush(Entity fatty, Entity target, float mult)
    {
        if (target.Center.X < fatty.Center.X && target.velocity.X > -6)
        {
            if (target.velocity.X > 0)
                target.velocity.X *= 0.98f;
            target.velocity.X -= Math.Min(0.15f * mult, 1f);
        }
        else if (target.Center.X > fatty.Center.X && target.velocity.X < 6)
        {
            if (target.velocity.X < 0)
                target.velocity.X *= 0.98f;
            target.velocity.X += Math.Min(0.15f * mult, 1f);
        }

        if (target.Center.Y < fatty.Center.Y - fatty.height / 4 && target.velocity.Y > -6)
        {
            if (target.velocity.Y > 0)
                target.velocity.Y *= 0.98f;
            target.velocity.Y -= Math.Min(0.15f * mult, 1f);
        }
        else if (target.Center.Y > fatty.Center.Y + fatty.height / 4 && target.velocity.Y < 6)
        {
            if (target.velocity.Y < 0)
                target.velocity.Y *= 0.98f;
            target.velocity.Y += Math.Min(0.15f * mult, 1f);
        }
    }

    public static void DoPush(Entity fatty, Entity target)
    {
        float sizeComparison = fatty.CompareSizeTo(target);
        if (sizeComparison > 1.75 && target.velocity.Length() > 40) // bounce away
        {
            Vector2 dir = fatty.Center.DirectionTo(target.Center);
            float velocity = target.velocity.Length();
            target.velocity = velocity * dir;
            if (target is Player playerTarget)
            {
                if (playerTarget.velocity.Y < -4)
                {
                    playerTarget.RefreshExtraJumps();
                    playerTarget.wingTime = playerTarget.wingTimeMax;
                }
            }
        }
        else if (sizeComparison > 1.15) // push away
        {
            ActuallyPush(fatty, target, sizeComparison);
        }
        else if (sizeComparison > 0.95)
        {
            ActuallyPush(fatty, target, 1);
            ActuallyPush(target, fatty, 1);
        }
    }

    public static void PushAwayFromMe(this Entity entity)
    {
        if (!entity.active)
            return;
        foreach (Player plr in Main.ActivePlayers)
        {
            if (!plr.active || plr.dead)
                continue;
            if (plr.Hitbox.Intersects(entity.Hitbox))
                DoPush(entity, plr);
        }
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (!npc.active)
                continue;
            if (npc.type == NPCID.TargetDummy)
                continue;
            if (npc.Hitbox.Intersects(entity.Hitbox))
                DoPush(entity, npc);
        }
    }
}

public class FatPushPlayer : ModPlayer
{
    public override void PostUpdateMiscEffects()
    {
        if (Player.Wg().Weight.GetStage() > 4)
            Player.PushAwayFromMe();
    }
}
