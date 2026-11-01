using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using WgMod.Content.Dusts;

namespace WgMod.Content.NPCs.Caverns;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
public class SugarSpecter : SweetSpirit
{
    public override int FrameCount => 13;
    public override int IdleFrames => 2;
    public override int WanderTime => 12 * 60;
    public override float WeightGain => 1f;

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 40;
        NPC.height = 40;
        NPC.damage = 40;
        NPC.defense = 18;
        NPC.lifeMax = 150;
        NPC.value = 100f;

        NPC.DeathSound = SoundID.NPCDeath6;
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Main.hardMode)
            return 0f;
        return SpawnCondition.Cavern.Chance * 0.04f;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        for (int i = 0; i < 15; i++)
        {
            Dust chunks = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<SugarSpecterplasm>(), NPC.velocity.X, NPC.velocity.Y, default, default, 1);
            chunks.noGravity = true;
        }

        if (NPC.life <= 0)
        {
            int bigChunk = Mod.Find<ModGore>($"{Name}_Chunk_Big").Type;
            int smallChunk = Mod.Find<ModGore>($"{Name}_Chunk_Small").Type;

            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(0, 40), Main.rand.Next(0, 40)), NPC.velocity, bigChunk);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(0, 40), Main.rand.Next(0, 40)), NPC.velocity, smallChunk);
            }
        }
    }

    public override Vector2 GetEnterOffset()
    {
        return new Vector2(40f, 21f);
    }

    public override void OnKill()
    {
        if (NPC.GetWereThereAnyInteractions())
            Utility.RegisterKill<SweetSpirit>();
    }
}
