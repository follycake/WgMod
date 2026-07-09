using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class MilkshakeSickness : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<MilkshakeSicknessPlayer>().MilkshakeSickness = true;
    }
}

public class MilkshakeSicknessPlayer : ModPlayer
{
    public bool MilkshakeSickness;

    const int TicksPerCycle = 30;
    const int FatPerCycle = 3;
    int _cooldown;

    public override void ResetEffects()
    {
        MilkshakeSickness = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!MilkshakeSickness || drawInfo.shadow != 0f)
            return;

        int dustRate = 5;
        int gurgleRate = 15;

        /*r = 50;
        g = 25;
        b = 20;*/

        if (_cooldown < TicksPerCycle)
            _cooldown++;
        else
        {
            _cooldown = 0;

            if (Main.rand.NextBool(dustRate))
            {

                int bubble = Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.BubbleBurst_Pink,
                    0f,
                    -1f,
                    100,
                    default,
                    2f
                );

                Main.dust[bubble].noGravity = true;

                SoundStyle gurgle = new SoundStyle("WgMod/Assets/Sounds/Gurgle_", 4)
                {
                    Volume = ((float)WgClientConfig.Instance.GurgleVolume) / 100,
                    PitchVariance = 0.08f,
                    MaxInstances = 4
                };

                if (Main.rand.NextBool(gurgleRate))
                    SoundEngine.PlaySound(gurgle, Player.Center);
            }
        }
    }
}
