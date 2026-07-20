using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using WgMod.Common;

namespace WgMod;

public class SpriteSet
{
    public const string BasePath = "SpriteSets";
    public const string JsonFileName = "Set.json";
    public const string DefaultSet = "Folly";

    public static SpriteSet Current { get; private set; }
    public static string[] FoundSets { get; private set; }

    public string Author = "Unknown";
    public int ArmCount;
    public Layer[] Layers = [];
    public Dictionary<int, Stage> Stages = [];

    [JsonIgnore] public int FrameCount { get; private set; }
    [JsonIgnore] public Layer[] ArmLayers { get; private set; }
    [JsonIgnore] public Layer[] ArmorLayers { get; private set; }

    [JsonIgnore] public bool UVArmor => ArmorLayers.Length > 0;
    [JsonIgnore] public int ArmorAltasWidth { get; private set; }
    [JsonIgnore] public int ArmorAltasHeight { get; private set; }

    public Stage GetStage(int stage)
    {
        if (Stages.TryGetValue(stage, out Stage result))
            return result;
        return Stage.Fallback;
    }

    public static void Initialize(Mod mod, string name)
    {
        Main.RunOnMainThread(() => WgArmorLUTs.Initialize(mod));
        FoundSets = [.. FindSets(mod)];
        if (!Exists(mod, name))
            name = DefaultSet;
        Current = Load(mod, name);
    }

    public static IEnumerable<string> FindSets(Mod mod)
    {
        foreach (string path in mod.GetFileNames())
        {
            if (Path.GetFileName(path) != JsonFileName)
                continue;
            string relative = Path.GetRelativePath(BasePath, Path.GetDirectoryName(path));
            if (relative.Contains(".."))
                continue;
            yield return relative;
        }
    }

    public static bool Exists(Mod mod, string name)
    {
        return mod.FileExists(Path.Combine(BasePath, name, JsonFileName));
    }

    public static SpriteSet Load(Mod mod, string name)
    {
        string path = Path.Combine(BasePath, name);
        SpriteSet set = JsonConvert.DeserializeObject<SpriteSet>(GetFileText(mod, Path.Combine(path, JsonFileName)));

        List<Layer> armorLayers = [];
        set.ArmorAltasWidth = 0;
        void LoadTextures(Layer layer, int lookup = 0)
        {
            layer.Texture = mod.Assets.Request<Texture2D>(Path.Combine(path, layer.Name));
            layer.ArmorAtlasX = set.ArmorAltasWidth;

            string armorName = Path.Combine(path, layer.Name + "_Armor");
            bool hasArmor = mod.HasAsset(armorName);
            bool simpleArmor = true;
            if (!hasArmor)
            {
                armorName = Path.Combine(path, layer.Name + "_ExArmor");
                hasArmor = mod.HasAsset(armorName);
                simpleArmor = false;
            }
            if (hasArmor)
            {
                layer.ArmorTexture = mod.Assets.Request<Texture2D>(armorName, AssetRequestMode.ImmediateLoad).Value;
                if (simpleArmor)
                    Main.RunOnMainThread(() => WgArmorLUTs.ConvertSimple(lookup, layer.ArmorTexture));
                set.ArmorAltasWidth += layer.ArmorTexture.Width;
                set.ArmorAltasHeight = Math.Max(set.ArmorAltasHeight, layer.ArmorTexture.Height);
                armorLayers.Add(layer);
            }
        }

        foreach (Layer layer in set.Layers)
            LoadTextures(layer);

        set.ArmLayers = new Layer[set.ArmCount];
        for (int i = 0; i < set.ArmLayers.Length; i++)
        {
            Layer arm = new() { Name = "Arms" + i, Type = LayerType.Arms };
            LoadTextures(arm, 1);
            set.ArmLayers[i] = arm;
        }

        int frame = 0;
        foreach (Stage stage in set.Stages.OrderBy(p => p.Key).Select(p => p.Value))
            stage.Frame = frame++;
        set.FrameCount = frame;

        set.ArmorLayers = [.. armorLayers];
        return set;
    }

    static string GetFileText(Mod mod, string path)
    {
        using Stream stream = mod.GetFileStream(path);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }

    public enum LayerType
    {
        Belly = 0,
        Legs,
        Breasts,
        Arms
    }

    public enum RenderType
    {
        Show = 0,
        Hide,
        FemaleOnly,
        MaleOnly
    }

    public class Layer
    {
        public string Name;
        public LayerType Type;
        public RenderType Render;

        [JsonIgnore] public Asset<Texture2D> Texture;
        [JsonIgnore] public Texture2D ArmorTexture;
        [JsonIgnore] public int ArmorAtlasX;

        [JsonIgnore] public bool UVArmor => ArmorTexture != null;

        public bool ShouldRender(Player player) => Render switch
        {
            RenderType.Show => true,
            RenderType.Hide => false,
            RenderType.FemaleOnly => !player.Male,
            RenderType.MaleOnly => player.Male,
            _ => true,
        };
    }

    public class Stage
    {
        public static readonly Stage Fallback = new();

        public int Arm = -1;
        public bool OnTop;
        public float OffsetX;
        public float OffsetY;

        [JsonIgnore] public int Frame;
    }
}
