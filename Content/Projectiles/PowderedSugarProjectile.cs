using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Systems;
using WgMod.Content.Achievements;
using WgMod.Content.NPCs.TownNPCs.GroundedHarpy;
using WgMod.Content.NPCs.TownNPCs.OverflowingMimic;

namespace WgMod.Content.Projectiles;

public class PowderedSugarProjectile : ModProjectile
{
    public static readonly HashSet<int> Mimics =
    [
        NPCID.Mimic,
        NPCID.IceMimic
    ];

    public override string Texture => "WgMod/Content/Dusts/CutieHeart";

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.PurificationPowder);
    }

    public override void PostAI()
    {
        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC npc = Main.npc[i];
            if (npc.active && npc.Hitbox.Intersects(Projectile.Hitbox) && ConvertNPC(npc))
            {
                npc.active = false;
                Reshaped.Condition.Complete();
                break;
            }
        }
    }

    static bool ConvertNPC(NPC npc)
    {
        if (npc.type == NPCID.Harpy && !TownNPCRespawnSystem.unlockGroundedHarpy)
        {
            NPC.NewNPC(Terraria.Entity.GetSource_TownSpawn(),
                (int)npc.Center.X,
                (int)npc.Center.Y,
                ModContent.NPCType<GroundedHarpyNPC>()
            );
            return true;
        }
        if (Mimics.Contains(npc.type) && !TownNPCRespawnSystem.unlockOverflowingMimic)
        {
            int npcId = NPC.NewNPC(
                Terraria.Entity.GetSource_TownSpawn(),
                (int)npc.Center.X,
                (int)npc.Center.Y,
                ModContent.NPCType<OverflowingMimicNPC>()
            );
            int variation;
            if (npc.type == NPCID.IceMimic)
                variation = 3;
            else
                variation = (int)(npc.ai[3] - 1f);
            Main.npc[npcId].townNpcVariationIndex = variation;
            return true;
        }
        return false;
    }
}
