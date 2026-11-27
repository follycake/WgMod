using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using WgMod.Content.Dusts;

namespace WgMod.Common.Systems;

// eventually put this somewhere else maybe
public class DumbParticleSystem : ModSystem
{
    public static void LardExplode(IEntitySource source, int count, Vector2 pos, float maxVelocity, bool bigger, bool lessDown)
    {
        Mod mod = ModContent.GetInstance<WgMod>();

        int gore_lard1 = mod.Find<ModGore>("Lard_Tiny").Type;
        int gore_lard2 = mod.Find<ModGore>("Lard_Medium").Type;
        int gore_lard3 = mod.Find<ModGore>("Lard_Big").Type;

        int dust_lard1 = ModContent.DustType<LardDustTiny>();
        int dust_lard2 = ModContent.DustType<LardDustMedium>();
        int dust_lard3 = ModContent.DustType<LardDustBig>();

        for (int i = 0; i < count; i++)
        {
            if (i % 2 == 0)
            {
                int usedIndex;

                if (bigger)
                    usedIndex = Main.rand.NextBool() ? gore_lard2 : gore_lard3;
                else
                    usedIndex = Main.rand.NextBool() ? gore_lard1 : gore_lard2;

                Gore.NewGore(source, pos, new Vector2(Main.rand.NextFloat(-maxVelocity, maxVelocity), Main.rand.NextFloat(-maxVelocity * 1.2f, lessDown ? 0f : maxVelocity)),
                    usedIndex, 1f);
            }
            else
            {
                int usedIndex;

                if (bigger)
                    usedIndex = Main.rand.NextBool() ? dust_lard2 : dust_lard3;
                else
                    usedIndex = Main.rand.NextBool() ? dust_lard1 : dust_lard2;
                Dust.NewDustPerfect(pos, usedIndex, new Vector2(Main.rand.NextFloat(-maxVelocity, maxVelocity), Main.rand.NextFloat(-maxVelocity * 1.2f, lessDown ? 0f : maxVelocity)));
            }
        }
    }
    public static void LardExplode(IEntitySource source, int count, Rectangle boxPos, float maxVelocity, bool bigger, bool lessDown)
    {
        Mod mod = ModContent.GetInstance<WgMod>();

        int gore_lard1 = mod.Find<ModGore>("Lard_Tiny").Type;
        int gore_lard2 = mod.Find<ModGore>("Lard_Medium").Type;
        int gore_lard3 = mod.Find<ModGore>("Lard_Big").Type;

        int dust_lard1 = ModContent.DustType<LardDustTiny>();
        int dust_lard2 = ModContent.DustType<LardDustMedium>();
        int dust_lard3 = ModContent.DustType<LardDustBig>();

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2(boxPos.X + Main.rand.Next(0, boxPos.Width + 1), boxPos.Y + Main.rand.Next(0, boxPos.Height + 1));
            if (i % 2 == 0)
            {
                int usedIndex;

                if (bigger)
                    usedIndex = Main.rand.NextBool() ? gore_lard2 : gore_lard3;
                else
                    usedIndex = Main.rand.NextBool() ? gore_lard1 : gore_lard2;

                Gore.NewGore(source, pos, new Vector2(Main.rand.NextFloat(-maxVelocity, maxVelocity), Main.rand.NextFloat(-maxVelocity * 1.2f, lessDown ? 0f : maxVelocity)),
                    usedIndex, 1f);
            }
            else
            {
                int usedIndex;

                if (bigger)
                    usedIndex = Main.rand.NextBool() ? dust_lard2 : dust_lard3;
                else
                    usedIndex = Main.rand.NextBool() ? dust_lard1 : dust_lard2;

                Dust.NewDustPerfect(pos, usedIndex, new Vector2(Main.rand.NextFloat(-maxVelocity, maxVelocity), Main.rand.NextFloat(-maxVelocity * 1.2f, lessDown ? 0f : maxVelocity)));
            }
        }
    }
}
