using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

public class SugarSpecterplasm : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
	}

	public override bool Update(Dust dust)
	{
		float light = 0.25f * dust.scale;

		Lighting.AddLight(dust.position, light * 2.55f, light * 1.74f, light * 1.82f);

		return true;
	}
}