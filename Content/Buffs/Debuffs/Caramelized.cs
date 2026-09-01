using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.sinnerdrip)]
public class Caramelized : ModBuff
{
    public static readonly int TagDamage = 10;

    public override void SetStaticDefaults()
    {
        BuffID.Sets.IsATagBuff[Type] = true;

        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        int dustRate = 15;
        if (Main.rand.NextBool(dustRate))
            Dust.NewDust(
                npc.position,
                npc.width,
                npc.height,
                DustID.t_Honey,
                0f,
                0.5f,
                150,
                new Color(151, 93, 15),
                1.3f
            );
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<CrispyDebuffPlayer>().CaramelizedEffect = true;

        int dustRate = 15;
        if (Main.rand.NextBool(dustRate))
            Dust.NewDust(
                player.position,
                player.width,
                player.height,
                DustID.t_Honey,
                0f,
                0.5f,
                150,
                new Color(151, 93, 15),
                1.3f
            );
    }
}

public class CaramelizedNPC : GlobalNPC
{
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
            return;
        var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
        if (npc.HasBuff<Caramelized>())
            modifiers.FlatBonusDamage += Caramelized.TagDamage * projTagMultiplier;
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (!npc.HasBuff<Caramelized>())
            return;
        damage = 5;
        if (npc.lifeRegen > 0)
            npc.lifeRegen = 0;
        npc.lifeRegen -= 20;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (!npc.HasBuff<Caramelized>())
            return;
        drawColor = new Color(151, 93, 15);
    }
}

public class CrispyDebuffPlayer : ModPlayer
{
    public bool CaramelizedEffect;

    public override void ResetEffects()
    {
        CaramelizedEffect = false;
    }

    public override void UpdateBadLifeRegen()
    {
        if (!CaramelizedEffect)
            return;
        if (Player.lifeRegen > 0)
            Player.lifeRegen = 0;
        Player.lifeRegenTime = 0;
        Player.lifeRegen -= 20;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!CaramelizedEffect)
            return;
        r = 151f / 255f;
        g = 93f / 255f;
        b = 15f / 255f;
    }
}
