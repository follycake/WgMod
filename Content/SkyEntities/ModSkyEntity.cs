using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Ambience;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Skies;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.Utilities;

namespace WgMod.Content.SkyEntities;

public abstract class ModSkyEntity : ModTexturedType
{
    public const SkyEntityType VanillaCount = SkyEntityType.Hellbats + 1;
    internal static readonly List<ModSkyEntity> _skyEntities = [];
    internal static readonly FastRandom _sharedFastRandom = new();

    // Common
    public int Type { get; private set; }
    public Asset<Texture2D> TextureAsset { get; private set; }
    public virtual bool IsHellEntity => false;

    // Instance
    public Player Player { get; private set; }
    public FastRandom Random { get; private set; } = _sharedFastRandom;

    public int Direction
    {
        get => (Effects & SpriteEffects.FlipHorizontally) != 0 ? -1 : 1;
        set
        {
            if (value < 0)
                Effects |= SpriteEffects.FlipHorizontally;
            else
                Effects &= ~SpriteEffects.FlipHorizontally;
        }
    }

    // SkyEntity
    public Vector2 Position;

    /// <summary> Defines the horizontal and vertical frame count. </summary>
    public SpriteFrame Frame;

    /// <summary> How far away the entity is. </summary>
    public float Depth;

    public SpriteEffects Effects;
    public bool IsActive;
    public float Rotation;
    public Rectangle SourceRectangle => Frame.GetSourceRectangle(TextureAsset.Value);

    // FadingSkyEntity
    /// <summary> How many ticks does the entity last. </summary>
    public int LifeTime;

    public Vector2 Velocity;

    /// <summary> How many ticks do frames take up. </summary>
    public int FramingSpeed;

    public int TimeEntitySpawnedIn;
    public float Opacity;

    /// <summary> How bright the entity is. Value between 0f and 1f. Defaults to 1f. </summary>
    public float BrightnessLerper = 1f;

    /// <summary> The maximum opacity the entity can reach. Value between 0f and 1f. Defaults to 1f. </summary>
    public float FinalOpacityMultiplier = 1f;

    /// <summary> Where to start fading in relative to <see cref="LifeTime"/>. Value between 0f and 1f. Defaults to 0.1f. </summary>
    public float OpacityNormalizedTimeToFadeIn = 0.1f;

    /// <summary> Where to start fading out relative to <see cref="LifeTime"/>. Value between 0f and 1f. Defaults to 0.9f. </summary>
    public float OpacityNormalizedTimeToFadeOut = 0.9f;

    /// <summary> Animation time offset. </summary>
    public int FrameOffset;

    public static bool IsPlayerAtRightHeightForType(int type, Player plr)
    {
        if (_skyEntities[type].IsHellEntity)
            return IsPlayerInAPlaceWhereTheyCanSeeAmbienceHell(plr);
        return IsPlayerInAPlaceWhereTheyCanSeeAmbienceSky(plr);
    }

    public static bool IsPlayerInAPlaceWhereTheyCanSeeAmbienceSky(Player plr)
    {
        return plr.position.Y <= Main.worldSurface * 16.0 + 1600.0;
    }

    public static bool IsPlayerInAPlaceWhereTheyCanSeeAmbienceHell(Player plr)
    {
        return plr.position.Y >= (Main.UnderworldLayer - 100) * 16;
    }

    public static void SpawnForPlayer(Player player, int type)
    {
        NetManager.Instance.BroadcastOrLoopback(NetAmbienceModule.SerializeSkyEntitySpawn(player, (SkyEntityType)((int)VanillaCount + type)));
    }

    public static void SpawnForPlayer<T>(Player player) where T : ModSkyEntity
    {
        SpawnForPlayer(player, ModContent.GetInstance<T>().Type);
    }

    internal static ModSkyEntity NewSkyEntity(int type, Player player, int seed)
    {
        ModSkyEntity clone = (ModSkyEntity)_skyEntities[type].MemberwiseClone();
        clone.Player = player;
        clone.Random = new FastRandom(seed);
        clone.IsActive = true;
        clone.TimeEntitySpawnedIn = -1;
        clone.BrightnessLerper = 1f;
        clone.FinalOpacityMultiplier = 1f;
        clone.OpacityNormalizedTimeToFadeIn = 0.1f;
        clone.OpacityNormalizedTimeToFadeOut = 0.9f;
        clone.Spawn();
        return clone;
    }

    /// <summary> Return null to use <see cref="SpawnChance"/>, return true to force spawn and false to never spawn. </summary>
    public virtual bool? ShouldSpawn()
    {
        return null;
    }

    public virtual float SpawnChance()
    {
        return 1f;
    }

    public virtual int SpawnCount()
    {
        return 1;
    }

    /// <summary> Do initialization here </summary>
    public virtual void Spawn()
    {
    }

    public virtual void Update(int frameCount)
    {
        if (!IsMovementDone(frameCount))
        {
            UpdateOpacity(frameCount);
            if ((frameCount + FrameOffset) % FramingSpeed == 0)
                NextFrame();
            UpdateVelocity(frameCount);
            Position += Velocity;
        }
    }

    /// <summary> Set <see cref="Velocity"/> here </summary>
    public virtual void UpdateVelocity(int frameCount)
    {
    }

    public virtual Color GetColor(Color backgroundColor)
    {
        return Color.Lerp(backgroundColor, Color.White, BrightnessLerper) * Opacity * FinalOpacityMultiplier * Helper_GetOpacityWithAccountingForOceanWaterLine();
    }

    public virtual Vector2 GetDrawPosition()
    {
        return Position;
    }

    public virtual void Draw(SpriteBatch spriteBatch, float depthScale, float minDepth, float maxDepth)
    {
        CommonDraw(spriteBatch, depthScale, minDepth, maxDepth);
    }

    public void CommonDraw(SpriteBatch spriteBatch, float depthScale, float minDepth, float maxDepth)
    {
        if (!(Depth <= minDepth) && !(Depth > maxDepth))
        {
            Vector2 drawPositionByDepth = GetDrawPositionByDepth();
            Color color = GetColor(Main.ColorOfTheSkies) * Main.atmo;
            Vector2 origin = SourceRectangle.Size() / 2f;
            float scale = depthScale / Depth;
            spriteBatch.Draw(TextureAsset.Value, drawPositionByDepth - Main.Camera.UnscaledPosition, SourceRectangle, color, Rotation, origin, scale, Effects, 0f);
        }
    }

    public void NextFrame()
    {
        Frame.CurrentRow = (byte)((Frame.CurrentRow + 1) % Frame.RowCount);
    }

    public void StartFadingOut(int currentFrameCount)
    {
        int fadeOutTime = (int)(LifeTime * OpacityNormalizedTimeToFadeOut);
        int num2 = currentFrameCount - fadeOutTime;
        if (num2 < TimeEntitySpawnedIn)
            TimeEntitySpawnedIn = num2;
    }

    public void SetPositionInWorldBasedOnScreenSpace(Vector2 actualWorldSpace)
    {
        Vector2 val = actualWorldSpace - Main.Camera.Center;
        Vector2 position = Main.Camera.Center + val * (Depth / 3f);
        Position = position;
    }

    void UpdateOpacity(int frameCount)
    {
        int time = frameCount - TimeEntitySpawnedIn;
        if (time >= LifeTime * OpacityNormalizedTimeToFadeOut)
            Opacity = Utils.GetLerpValue(LifeTime, LifeTime * OpacityNormalizedTimeToFadeOut, time, true);
        else
            Opacity = Utils.GetLerpValue(0f, LifeTime * OpacityNormalizedTimeToFadeIn, time, true);
    }

    bool IsMovementDone(int frameCount)
    {
        if (TimeEntitySpawnedIn == -1)
            TimeEntitySpawnedIn = frameCount;
        if (frameCount - TimeEntitySpawnedIn >= LifeTime)
        {
            IsActive = false;
            return true;
        }
        return false;
    }

    Vector2 GetDrawPositionByDepth()
    {
        return (GetDrawPosition() - Main.Camera.Center) * new Vector2(1f / Depth, 0.9f / Depth) + Main.Camera.Center;
    }

    float Helper_GetOpacityWithAccountingForOceanWaterLine()
    {
        Vector2 val = GetDrawPositionByDepth() - Main.Camera.UnscaledPosition;
        int num = SourceRectangle.Height / 2;
        float t = val.Y + num;
        float yScreenPosition = AmbientSkyDrawCache.Instance.OceanLineInfo.YScreenPosition;
        float lerpValue = Utils.GetLerpValue(yScreenPosition - 10f, yScreenPosition - 2f, t, true);
        lerpValue *= AmbientSkyDrawCache.Instance.OceanLineInfo.OceanOpacity;
        return 1f - lerpValue;
    }

    protected sealed override void Register()
    {
        ModTypeLookup<ModSkyEntity>.Register(this);
        Type = _skyEntities.Count;
        _skyEntities.Add(this);
    }

    public sealed override void SetupContent()
    {
        TextureAsset = ModContent.Request<Texture2D>(Texture);
        SetStaticDefaults();
    }
}

public class ModSkyEntitySystem : ModSystem
{
    static FieldInfo _frameCounter;
    static FieldInfo _updatesUntilNextAttempt;

    internal static readonly List<ModSkyEntity> _entities = [];
    static readonly List<Player> _foundPlayers = [];
    static readonly WeightedRandom<int> _typeBag = new();

    public override void Load()
    {
        _frameCounter = typeof(AmbientSky).GetField(nameof(_frameCounter), BindingFlags.Instance | BindingFlags.NonPublic);
        _updatesUntilNextAttempt = typeof(AmbienceServer).GetField(nameof(_updatesUntilNextAttempt), BindingFlags.Instance | BindingFlags.NonPublic);
        On_AmbientSky.Update += Update;
        On_AmbientSky.Draw += Draw;
        On_AmbientSky.Spawn += Spawn;
        On_AmbienceServer.Update += AmbienceUpdate;
    }

    public override void Unload()
    {
        On_AmbientSky.Update -= Update;
        On_AmbientSky.Draw -= Draw;
        On_AmbientSky.Spawn -= Spawn;
        On_AmbienceServer.Update -= AmbienceUpdate;
        _entities.Clear();
    }

    static void Update(On_AmbientSky.orig_Update orig, AmbientSky self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (Main.gamePaused)
            return;
        int frameCounter = (int)_frameCounter.GetValue(self);
        foreach (ModSkyEntity entity in _entities)
            entity.Update(frameCounter);
        for (int i = _entities.Count - 1; i >= 0; i--)
        {
            if (!_entities[i].IsActive)
                _entities.RemoveAt(i);
        }
        if (Main.netMode != NetmodeID.Server && _entities.Count > 0 && !SkyManager.Instance["Ambience"].IsActive())
            SkyManager.Instance.Activate("Ambience");
        if (Main.netMode != NetmodeID.Server && _entities.Count == 0 && SkyManager.Instance["Ambience"].IsActive())
            SkyManager.Instance.Deactivate("Ambience");
    }

    static void Draw(On_AmbientSky.orig_Draw orig, AmbientSky self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (Main.gameMenu && Main.netMode == NetmodeID.SinglePlayer && SkyManager.Instance["Ambience"].IsActive())
            _entities.Clear();
        orig(self, spriteBatch, minDepth, maxDepth);
        foreach (ModSkyEntity entity in _entities)
            entity.Draw(spriteBatch, 3f, minDepth, maxDepth);
    }

    static void Spawn(On_AmbientSky.orig_Spawn orig, AmbientSky self, Player player, SkyEntityType type, int seed)
    {
        const SkyEntityType count = SkyEntityType.Hellbats + 1;
        if (type >= count)
            _entities.Add(ModSkyEntity.NewSkyEntity(type - count, player, seed));
        orig(self, player, type, seed);
    }

    static void AmbienceUpdate(On_AmbienceServer.orig_Update orig, AmbienceServer self)
    {
        orig(self);
        if ((double)_updatesUntilNextAttempt.GetValue(self) > 0.0)
            return;
        _foundPlayers.Clear();
        for (int i = 0; i < Main.maxPlayers; i++)
        {
            Player player = Main.player[i];
            if (player.active && (ModSkyEntity.IsPlayerInAPlaceWhereTheyCanSeeAmbienceSky(player) || ModSkyEntity.IsPlayerInAPlaceWhereTheyCanSeeAmbienceHell(player)))
                _foundPlayers.Add(player);
        }
        if (_foundPlayers.Count > 0)
        {
            Player target = _foundPlayers[Main.rand.Next(_foundPlayers.Count)];
            _typeBag.Clear();
            _typeBag.Add(-1, 1.0);
            int count;
            for (int i = 0; i < ModSkyEntity._skyEntities.Count; i++)
            {
                ModSkyEntity entity = ModSkyEntity._skyEntities[i];
                if (entity.ShouldSpawn() is bool forceSpawn)
                {
                    if (forceSpawn)
                    {
                        count = entity.SpawnCount();
                        for (int j = 0; j < count; j++)
                            ModSkyEntity.SpawnForPlayer(target, i);
                    }
                }
                else
                    _typeBag.Add(i, entity.SpawnChance());
            }
            int type = _typeBag;
            if (type < 0)
                return;
            count = ModSkyEntity._skyEntities[type].SpawnCount();
            for (int i = 0; i < count; i++)
                ModSkyEntity.SpawnForPlayer(target, type);
        }
    }
}
