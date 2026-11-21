using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Projectiles;

public abstract class TargetProjectile : ModProjectile
{
    public static void SearchForTargets(Player owner, Projectile projectile, float range, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
    {
        distanceFromTarget = range;
        targetCenter = projectile.position;
        foundTarget = false;

        if (owner.HasMinionAttackTargetNPC)
        {
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            float between = Vector2.Distance(npc.Center, projectile.Center);

            if (between < range)
            {
                distanceFromTarget = between;
                targetCenter = npc.Center;
                foundTarget = true;
            }
        }

        if (!foundTarget)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    float between = Vector2.Distance(npc.Center, projectile.Center);
                    bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                    bool inRange = between < distanceFromTarget;
                    bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                    bool closeThroughWall = between < 100f;

                    if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                    }
                }
            }
        }

        projectile.friendly = foundTarget;
    }
}