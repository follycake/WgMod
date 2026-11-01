using Terraria;
using Terraria.ModLoader;

namespace WgMod.Common.Players;

public class StatPlayer : ModPlayer
{
    // yknow i kinda expected to come up with more stuff but nah
    public float CritDamage;

    public override void ResetEffects()
    {
        CritDamage = 0f;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage += CritDamage;
    }
}
