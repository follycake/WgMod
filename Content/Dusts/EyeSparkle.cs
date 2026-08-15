using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class EyeSparkle : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
    }

    public override bool Update(Dust dust)
    {
        dust.fadeIn += 1f / 60f;

        if (dust.fadeIn > 1f)
            dust.active = false;

        dust.frame = new Rectangle(0, (int)(dust.fadeIn * 9) * 30, 30, 30);

        float light = 0.35f * dust.scale;

        Lighting.AddLight(dust.position, light * 2, light, light * 2);

        return false;
    }
}
