using Terraria.Achievements;
using Terraria.ModLoader;

namespace WgMod.Content.Achievements;

public class LetsGetPhysical : ModAchievement
{
	public static AchievementCondition Condition { get; private set; }

	public override void SetStaticDefaults()
	{
		Achievement.SetCategory(AchievementCategory.Challenger);

		Condition = AddCondition();
	}
}
