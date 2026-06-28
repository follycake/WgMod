using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Systems;
using WgMod.Content.NPCs.GroundedHarpy;
using WgMod.Content.Projectiles;

namespace WgMod.Common.GlobalNPCs;

public class TransformGlobalNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        if (npc.type == NPCID.Harpy && TownNPCRespawnSystem.unlockGroundedHarpy == false)
        {
            for (int i = 0; i < 300; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<PowderedSugarProjectile>() && Vector2.Distance(npc.Center, proj.Center) < npc.height)
                {
                    NPC.NewNPC(
                        Entity.GetSource_TownSpawn(),
                        (int)npc.Center.X,
                        (int)npc.Center.Y,
                        ModContent.NPCType<GroundedHarpyNPC>()
                    );
                    npc.active = false;
                }
            }
        }
    }
}
