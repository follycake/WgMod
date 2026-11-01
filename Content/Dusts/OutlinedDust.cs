using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Dusts;

// i know theres like. Outline shaders and stuff. BUt i dont care about that

public class OutlinedDustSmall : ModDust
{
    public override string Texture => "WgMod/Content/Dusts/OutlinedDust";

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = false;
        dust.frame = new Rectangle(0, 0, 6, 6);
        dust.alpha = 0;
    }

    public override bool PreDraw(Dust dust)
    {
        return false;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.velocity *= 0.97f;

        dust.scale *= 0.97f;

        float light = dust.scale * 0.001f;

        Lighting.AddLight(dust.position, new Vector3(dust.color.R * light, dust.color.G * light, dust.color.B * light));

        if (dust.scale <= 0.15f)
            dust.active = false;

        return false;
    }
}

public class OutlinedDustBig : ModDust
{
    public override string Texture => "WgMod/Content/Dusts/OutlinedDust";

    static void DrawOutlinedDust(On_Main.orig_DrawDust orig, Main self)
    {
        Texture2D sprite = ModContent.Request<Texture2D>("WgMod/Content/Dusts/OutlinedDust").Value;

        Main.spriteBatch.Begin(0, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

        for (int i = 0; i < Main.maxDustToDraw; i++)
        {
            Dust dust = Main.dust[i];
            if (!dust.active)
                continue;
            if (dust.type == ModContent.DustType<OutlinedDustSmall>())
                Main.spriteBatch.Draw(sprite, dust.position - Main.screenPosition, new Rectangle(6, 0, 6, 6), dust.color, dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
            else if (dust.type == ModContent.DustType<OutlinedDustBig>())
                Main.spriteBatch.Draw(sprite, dust.position - Main.screenPosition, new Rectangle(7, 7, 7, 7), dust.color, dust.rotation, new Vector2(3.5f, 3.5f), dust.scale, SpriteEffects.None, 0f);
        }

        for (int i = 0; i < Main.maxDustToDraw; i++)
        {
            Dust dust = Main.dust[i];
            if (!dust.active)
                continue;
            if (dust.type == ModContent.DustType<OutlinedDustSmall>())
                Main.spriteBatch.Draw(sprite, dust.position - Main.screenPosition, new Rectangle(0, 0, 6, 6), Color.White, dust.rotation, new Vector2(3, 3), dust.scale, SpriteEffects.None, 0f);
            else if (dust.type == ModContent.DustType<OutlinedDustBig>())
                Main.spriteBatch.Draw(sprite, dust.position - Main.screenPosition, new Rectangle(0, 7, 7, 7), Color.White, dust.rotation, new Vector2(3.5f, 3.5f), dust.scale, SpriteEffects.None, 0f);
        }

        Main.spriteBatch.End();

        orig(self);
    }

    public override void Load()
    {
        On_Main.DrawDust += DrawOutlinedDust;
    }

    public override void Unload()
    {
        On_Main.DrawDust -= DrawOutlinedDust;
    }

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.noLight = false;
        dust.frame = new Rectangle(0, 0, 7, 7);
        dust.alpha = 0;
    }

    public override bool PreDraw(Dust dust)
    {
        return false;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.velocity *= 0.9f;

        dust.scale *= 0.98f;

        float light = dust.scale * 0.001f;

        Lighting.AddLight(dust.position, new Vector3(dust.color.R * light, dust.color.G * light, dust.color.B * light));

        if (dust.scale <= 0.15f)
            dust.active = false;

        return false;
    }
}
