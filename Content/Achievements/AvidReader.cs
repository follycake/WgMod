using Terraria.Achievements;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Magic;

namespace WgMod.Content.Achievements;

public class AvidReader : ModAchievement
{
	public override void SetStaticDefaults()
	{
		Achievement.SetCategory(AchievementCategory.Collector);

		AddItemCraftCondition(ModContent.ItemType<LiftingTome>());
	}

	public override Position GetDefaultPosition() => new After("MINER_FOR_FIRE");
	public override Position GetAdvisorPosition() => new After("MINER_FOR_FIRE");
}
