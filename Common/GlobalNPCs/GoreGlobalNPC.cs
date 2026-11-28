using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Common.GlobalNPCs;

public class GoreGlobalNPC : GlobalNPC
{
    public void TownNPCDustAndGore(NPC npc, string name, int particle, int amount = 15)
    {
        GlobalNPCDust(npc, particle, amount);

        if (Main.netMode != NetmodeID.Server && npc.life <= 0)
        {
            string variant = "";

            if (npc.IsShimmerVariant)
                variant += "_Shimmer";

            if (npc.altTexture == 1)
                variant += "_Party";

            int headGore = Mod.Find<ModGore>($"{name}_Gore{variant}_Head").Type;
            int armGore = Mod.Find<ModGore>($"{name}_Gore{variant}_Arm").Type;
            int legGore = Mod.Find<ModGore>($"{name}_Gore{variant}_Leg").Type;

            Gore.NewGore(npc.GetSource_Death(), npc.position, npc.velocity, headGore, 1f);
            Gore.NewGore(npc.GetSource_Death(), npc.position + new Vector2(0, 20), npc.velocity, armGore);
            Gore.NewGore(npc.GetSource_Death(), npc.position + new Vector2(0, 20), npc.velocity, armGore);
            Gore.NewGore(npc.GetSource_Death(), npc.position + new Vector2(0, 34), npc.velocity, legGore);
            Gore.NewGore(npc.GetSource_Death(), npc.position + new Vector2(0, 34), npc.velocity, legGore);
        }
    }

    public void GlobalNPCDust(NPC npc, int dust, int amount = 15)
    {
        for (int k = 0; k < amount; k++)
            Dust.NewDust(npc.position, npc.width, npc.height, dust);
    }
}