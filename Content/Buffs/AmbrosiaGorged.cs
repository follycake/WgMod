using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.ubulumn)]
public class AmbrosiaGorged : ModBuff
{
    WgStat _moveSpeed = new(1.25f, 1.5f);
    WgStat _defense = new(1f, 10f);
    WgStat _regen = new(1f, 5f);

    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _moveSpeed.Lerp(immobility);
        _defense.Lerp(immobility);
        _regen.Lerp(immobility);

        player.moveSpeed *= _moveSpeed;
        player.maxRunSpeed *= _moveSpeed;
        player.runAcceleration *= _moveSpeed;
        player.accRunSpeed *= _moveSpeed;
        player.statDefense += _defense;
        player.lifeRegen += _regen;

        int dustRate = 10;
        if (Main.rand.NextBool(dustRate))
        {
            Dust.NewDust(
                player.position,
                player.width,
                player.height,
                DustID.t_Honey,
                0f,
                0.5f,
                150,
                default,
                1.3f
            );
        }
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = base.Description.Format((_moveSpeed - 1f).Percent(), _defense, _regen);
    }
}
