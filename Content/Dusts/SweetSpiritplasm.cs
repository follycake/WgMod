using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

public class SweetSpiritplasm : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
	}

	public override bool Update(Dust dust)
	{
		float light = 0.35f * dust.scale;

		Lighting.AddLight(dust.position, light, light, light);

		return true;
	}
}