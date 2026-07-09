using Terraria.Audio;
using WgMod.Common.Configs;

namespace WgMod;

[Credit(ProjectRole.SFX, Contributor.follycake)]
[Credit(ProjectRole.SFX, Contributor.purple_circle)]
public static class WgSounds
{
    public static readonly SoundStyle Belly = new("WgMod/Assets/Sounds/Belly_", 3, SoundType.Sound);
    public static readonly SoundStyle Gulp = new("WgMod/Assets/Sounds/Gulp_", 4, SoundType.Sound);
    public static readonly SoundStyle Stomp = new("WgMod/Assets/Sounds/Stomp_", 5, SoundType.Sound);
    public static readonly SoundStyle Squeaky = new("WgMod/Assets/Sounds/Squeaky", SoundType.Sound);

    static readonly SoundStyle _gurgle = new("WgMod/Assets/Sounds/Gurgle_", 4, SoundType.Sound)
    {
        PitchVariance = 0.08f,
        MaxInstances = 4
    };

    public static SoundStyle Gurgle => _gurgle.WithVolumeScale(WgClientConfig.Instance.GurgleVolume / 100f);
}
