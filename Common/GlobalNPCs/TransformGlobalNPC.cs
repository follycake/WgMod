using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Systems;
using WgMod.Content.NPCs.TownNPCs.GroundedHarpy;
using WgMod.Content.NPCs.TownNPCs.OverflowingMimic;
using WgMod.Content.Projectiles;

namespace WgMod.Common.GlobalNPCs;

public class TransformGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public List<int> mimics =
    [
        NPCID.Mimic,
        NPCID.IceMimic,
    ];

    public override void PostAI(NPC npc)
    {
        if (npc.type == NPCID.Harpy && !TownNPCRespawnSystem.unlockGroundedHarpy)
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

        if (mimics.Contains(npc.type) && !TownNPCRespawnSystem.unlockOverflowingMimic)
        {
            for (int i = 0; i < 300; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<PowderedSugarProjectile>() && Vector2.Distance(npc.Center, proj.Center) < npc.height)
                {
                    int npcId = NPC.NewNPC(
                        Entity.GetSource_TownSpawn(),
                        (int)npc.Center.X,
                        (int)npc.Center.Y,
                        ModContent.NPCType<OverflowingMimicNPC>()
                    );

                    int fuck;

                    if (npc.type == NPCID.IceMimic)
                        fuck = 3;
                    else
                        fuck = (int)(npc.ai[3] - 1f);

                    Main.npc[npcId].townNpcVariationIndex = fuck;

                    npc.active = false;
                }
            }
        }
    }
}
