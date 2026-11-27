using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;
public class LardDustTiny : ModDust
{
    public override string Texture => "WgMod/Content/NPCs/Gores/Lard_Tiny";

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = true;
        dust.frame = new Rectangle(0, 0, 6, 6);
    }

    public override bool Update(Dust dust)
    {
        dust.velocity = new Vector2(dust.velocity.X * 0.998f, dust.velocity.Y + 0.2f);

        dust.position += dust.velocity;

        dust.alpha += 2;

        if (dust.alpha >= 255)
            dust.active = false;

        return false;
    }
}

public class LardDustMedium : ModDust
{
    public override string Texture => "WgMod/Content/NPCs/Gores/Lard_Medium";

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = true;
        dust.frame = new Rectangle(0, 0, 8, 8);
    }

    public override bool Update(Dust dust)
    {
        dust.velocity = new Vector2(dust.velocity.X * 0.998f, dust.velocity.Y + 0.2f);

        dust.position += dust.velocity;

        dust.alpha += 2;

        if (dust.alpha >= 255)
            dust.active = false;

        return false;
    }
}

public class LardDustBig : ModDust
{
    public override string Texture => "WgMod/Content/NPCs/Gores/Lard_Big";

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = true;
        dust.frame = new Rectangle(0, 0, 12, 12);
    }

    public override bool Update(Dust dust)
    {
        dust.velocity = new Vector2(dust.velocity.X * 0.998f, dust.velocity.Y + 0.2f);

        dust.position += dust.velocity;

        dust.alpha += 2;

        if (dust.alpha >= 255)
            dust.active = false;

        return false;
    }
}
