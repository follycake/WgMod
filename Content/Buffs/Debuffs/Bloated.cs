using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs.Debuffs;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.PLACEHOLDER)]
public class Bloated : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }
}

public class BloatedPlayer : ModPlayer
{
    /// <summary> Whether or not the player has this buff. </summary>
    bool _active;
    /// <summary> Whether or not the player had this buff on the previous frame. </summary>
    bool _activePrevious;
    Mass _mass;

    public override void PostUpdateBuffs()
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg) || Player != Main.LocalPlayer)
            return;

        _activePrevious = _active;
        _active = Player.HasBuff<Bloated>();
        // checking stuff

        //Main.NewText($"prev: {_activePrevious} current: {_active}");

        if (_active && !_activePrevious)
            _mass = wg.AddWeight(200f);

        if (!_active && _activePrevious)
            wg.AddWeight(-_mass);
    }
}
