using Terraria.ModLoader;
using WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;

namespace WgMod.Content.Achievements;

public class GorgeistBeaten : ModAchievement
{
    public override void SetStaticDefaults()
    {
        AddNPCKilledCondition(ModContent.NPCType<Gorgeist>());
    }

    public override Position GetDefaultPosition() => new Before("EYE_ON_YOU");

    public override Position GetAdvisorPosition() => new Before("EYE_ON_YOU");
}
