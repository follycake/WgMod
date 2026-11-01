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
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;

        BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BoneJavelin);
    }
}

public class SunrisingNPC : GlobalNPC
{
    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!npc.HasBuff<Sunrising>())
            return;
        int sunriseCount = 0;
        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.type == ModContent.ProjectileType<SunriseProjectile>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                sunriseCount++;
            npc.lifeRegen -= sunriseCount * 2 * 3;
            if (damage < sunriseCount * 3)
                damage = sunriseCount * 3;
        }
    }
}
