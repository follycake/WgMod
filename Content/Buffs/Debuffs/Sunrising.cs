using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Projectiles.Melee;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.follycake)]
public class Sunrising : ModBuff
{
    public override void SetStaticDefaults()
    {
        BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<SunrisingNPC>()._active = true;
    }
}

public class SunrisingNPC : GlobalNPC
{
    public bool _active;

    public override void ResetEffects(NPC npc)
    {
        _active = false;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!_active)
            return;

        int sunriseCount = 0;

        foreach (var p in Main.ActiveProjectiles)
        {
            if (p.type == ModContent.ProjectileType<SunriseProjectile>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
            {
                sunriseCount++;
            }

            npc.lifeRegen -= sunriseCount * 2 * 3;

            if (damage < sunriseCount * 3)
                damage = sunriseCount * 3;
        }
    }
    public override bool InstancePerEntity => true;
}
