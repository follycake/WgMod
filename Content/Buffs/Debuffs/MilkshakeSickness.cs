using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
    public const int TicksPerCycle = 30;
    public bool MilkshakeSickness;

    int _cooldown;
    bool _requestDust;

    public override void ResetEffects()
    {
        MilkshakeSickness = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!MilkshakeSickness || drawInfo.shadow != 0f)
            return;
        if (_requestDust)
        {
            _requestDust = false;
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
        }
    }

    public override void PostUpdate()
    {
        if (!MilkshakeSickness)
            return;

        const int dustRate = 5;
        const int gurgleRate = 15;

        if (_cooldown < TicksPerCycle)
            _cooldown++;
        else
        {
            _cooldown = 0;
            if (Main.rand.NextBool(dustRate))
            {
                _requestDust = true;
                if (Player.whoAmI == Main.myPlayer && Main.rand.NextBool(gurgleRate))
                    Player.Wg().PlaySound(WgSounds.Gurgle);
            }
        }
    }
}
