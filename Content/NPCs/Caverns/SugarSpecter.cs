using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

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
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!Main.hardMode)
            return 0f;
        return SpawnCondition.Cavern.Chance * 0.04f;
    }

    public override Vector2 GetEnterOffset()
    {
        return new Vector2(40f, 21f);
    }
}
