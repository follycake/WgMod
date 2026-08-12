using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common.Configs;

namespace WgMod;

// TODO: Use calories instead
public record struct GainOptions(Mass TotalGain, float Time = 0f)
{
    public readonly bool IsInstant => Time < 0.01f;

    public static implicit operator GainOptions(float mass) => new(mass);
    public static implicit operator GainOptions(Mass mass) => new(mass);
}

// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
public partial class WgMod : Mod
{
    internal static readonly Dictionary<int, GainOptions> _buffTable = [];

    // Vanilla
    static void AddBuffs((int id, GainOptions gain)[] table)
    {
        foreach (var (id, gain) in table)
            _buffTable[id] = gain;
    }

    // Mods
    void AddBuffs(string mod, (string name, GainOptions gain)[] table)
    {
        if (!ModLoader.TryGetMod(mod, out Mod foundMod))
            return;
        foreach (var (name, gain) in table)
        {
            if (!foundMod.TryFind(name, out ModBuff foundBuff))
            {
                Logger.Warn($"Couldn't find buff '{name}' for mod '{mod}'");
                continue;
            }
            _buffTable[foundBuff.Type] = gain;
        }
    }

    public override void Load()
    {
        Credits.Scan(this);
        RegisterBuffs();
        RegisterHooks();

        if (!Main.dedServ)
            SpriteSet.Initialize(this, WgClientConfig.Instance.PlayerSpriteSet);
    }

    public override void Unload()
    {
        UnregisterHooks();
    }
}
