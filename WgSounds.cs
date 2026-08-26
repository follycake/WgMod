using System.Collections.Generic;
using Terraria.Audio;
using WgMod.Common.Configs;

namespace WgMod;

public enum VolumeChannel
{
    Misc = 0,
    Belly,
    Gurgle
}

[Credit(ProjectRole.SFX, Contributor.follycake)]
[Credit(ProjectRole.SFX, Contributor.purple_circle)]
public static class WgSounds
{
    public static readonly List<WgSound> AllSounds = [];

    // follycake
    public static readonly WgSound Belly = new("WgMod/Assets/Sounds/Belly_", 3, VolumeChannel.Belly);
    public static readonly WgSound Thump = new("WgMod/Assets/Sounds/Thump") { MaxInstances = 4, PitchVariance = 0.1f };

    // purple_circle
    public static readonly WgSound Gulp = new("WgMod/Assets/Sounds/Gulp_", 4);
    public static readonly WgSound Stomp = new("WgMod/Assets/Sounds/Stomp_", 5);
    public static readonly WgSound Squeaky = new("WgMod/Assets/Sounds/Squeaky");
    public static readonly WgSound Gurgle = new("WgMod/Assets/Sounds/Gurgle_", 4, VolumeChannel.Gurgle) { PitchVariance = 0.08f };
}

public class WgSound
{
    public readonly int Id;

    public string SoundPath;
    public int MaxInstances = 1;
    public int NumVariants = 1;
    public float PitchVariance = 0f;
    public VolumeChannel Channel = VolumeChannel.Misc;

    public WgSound()
    {
        Id = WgSounds.AllSounds.Count;
        WgSounds.AllSounds.Add(this);
    }

    public WgSound(string soundPath, int numVariants = 1, VolumeChannel channel = VolumeChannel.Misc) : this()
    {
        SoundPath = soundPath;
        NumVariants = numVariants;
        Channel = channel;
    }

    public SoundStyle Build(float volume = 1f) => new(SoundPath, NumVariants)
    {
        MaxInstances = MaxInstances,
        PitchVariance = PitchVariance,
        Volume = WgClientConfig.Instance.GetVolume(Channel) / 100f * volume
    };

    public static implicit operator SoundStyle(WgSound sound) => sound.Build();
}
