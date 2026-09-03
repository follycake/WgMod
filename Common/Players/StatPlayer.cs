using Terraria;
using Terraria.ModLoader;

namespace WgMod.Common.Players;
public class StatPlayer : ModPlayer
{
    //yknow i kinda expected to come up with more stuff but nah
    public float _critDamage;
    public override void ResetEffects()
    {
        _critDamage = 0f;
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage += _critDamage;
    }
}
