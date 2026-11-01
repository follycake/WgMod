using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class CrystalSugarFrenzy : ModBuff
{
    WgStat _speed = new(0.15f, 0.3f);

    public override void Update(Player player, ref int buffIndex)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _speed.Lerp(immobility);

        player.pickSpeed -= _speed;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        tip = base.Description.Format(_speed.Percent());
    }
}
