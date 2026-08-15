using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Systems;
using WgMod.Content.Buffs;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Common.Players;

public partial class WgPlayer : ModPlayer
{
    public const int DigestTime = 60;
    public const float DigestAmount = 0.25f;
    public const float StomachCapacity = 20f;

    /// <summary> The player's weight </summary>
    public Weight Weight { get; private set; } = Weight.Base;

    // <summary> The to be digested mass inside the player's stomach. Only relevant for the local client. </summary>
    public Mass Stomach { get; private set; }

    /// <summary> How much movement will be reduced because of the player's weight. Multiply this. </summary>
    public StatModifier MovementPenalty;

    /// <summary> How fast the player will lose weight from movement. Add or subtract to this. </summary>
    public StatModifier MovementWeightLossRate;

    /// <summary> How fast the player will gain weight from most sources. Add or subtract to this. </summary>
    public StatModifier WeightGainRate;

    /// <summary> How much weight the player will gain due to food. Multiply this. </summary>
    public StatModifier FoodAbsorption;

    /// <summary> The maximum weight stage that the player can reach </summary>
    public int MaxStage;

    /// <summary> Whether the weight is currently fixed/pinned. No gain or loss. </summary>
    public bool WeightFixed;

    /// <summary> Whether the player has jumped this tick. </summary>
    public bool JustJumped { get; private set; }

    public readonly int[] BuffDuration = new int[Player.MaxBuffs];
    internal int _ignoreWgBuffTimer = 2;

    internal float _finalKnockbackResistance;
    internal float _finalMovementFactor = 1f;
    internal int _finalMaxStage = WeightStage.Max;
    internal bool _finalWeightFixed;

    internal float _buffTotalGain;
    internal int _iceBreakTimer;
    internal bool _displayWeight;

    Vector2 _prevVel;
    int _digestTimer;
    bool _hasJumped;

    public override void Initialize()
    {
        SetWeightForced(Weight.Base, false);
    }

    public override void OnEnterWorld()
    {
        _ignoreWgBuffTimer = 2;
    }

    public bool OwnsPlayer()
    {
        return !Main.dedServ && Player.whoAmI == Main.myPlayer;
    }

    public void SetWeight(Weight weight, bool effects = true)
    {
        if (!OwnsPlayer() || _finalWeightFixed)
            return;
        if (WgClientConfig.Instance.DisableWeightGain)
            weight = new Weight(MathF.Min(weight.Mass, Weight.Mass));
        weight = Weight.Clamp(weight, _finalMaxStage);
        SetWeightForced(weight, effects);
    }

    public Mass AddWeight(Mass mass, bool effects = true)
    {
        Weight start = Weight;
        if (mass > 0f)
            mass = WeightGainRate.ApplyTo(mass);
        SetWeight(Weight + mass, effects);
        return Weight.Mass - start.Mass;
    }

    /// <summary> Do not use this unless you know what you're doing </summary>
    internal void SetWeightForced(Weight weight, bool effects = true)
    {
        int prevStage = Weight.GetStage();
        Weight = weight;
        if (Weight.GetStage() != prevStage && effects)
        {
            SoundEngine.PlaySound(WgSounds.Belly, Player.Center);
            Jiggle(3.6f);
        }
    }

    public void SetStomach(Mass mass, bool effects = true)
    {
        if (!OwnsPlayer())
            return;
        if (WgClientConfig.Instance.DisableWeightGain)
            mass = MathF.Min(mass, Stomach);
        SetStomachForced(mass, effects);
    }

    public Mass AddStomach(Mass mass, bool effects = true)
    {
        Mass start = Stomach;
        SetStomach(Stomach + mass, effects);
        return Stomach - start;
    }

    internal void SetStomachForced(Mass mass, bool effects = true)
    {
        Stomach = Math.Clamp(mass, 0f, StomachCapacity);
        if (mass > StomachCapacity)
            AddWeight(mass - StomachCapacity, effects);
    }

    public override void ResetEffects()
    {
        // Custom stats
        MovementPenalty = StatModifier.Default;
        MovementWeightLossRate = StatModifier.Default;
        WeightGainRate = StatModifier.Default;
        FoodAbsorption = StatModifier.Default;
        MaxStage = WgClientConfig.Instance.StageCap;

        _finalWeightFixed = WeightFixed;
        WeightFixed = false;

        // Jump detection
        if (Player.jump <= 0)
            _hasJumped = false;
        JustJumped = false;
        if (!_hasJumped && Player.jump > 0)
        {
            JustJumped = true;
            _hasJumped = true;
        }
    }

    public override void PreUpdateBuffs()
    {
        EnsureBuff<FatBuff>();
        EnsureBuff<StomachBuff>();
        if (Weight.GetStage() >= WeightStage.HardImmobile)
            Player.AddBuff(ModContent.BuffType<Tired>(), 2);
    }

    public void EnsureBuff<T>(int time = 60) where T : ModBuff
    {
        int type = ModContent.BuffType<T>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, time);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (WgServerConfig.Instance.DisableFatBuffs)
        {
            _finalMovementFactor = 1f;
            return;
        }

        const float mountReduction = 0.8f;
        if (Player.mount.Active)
            MovementPenalty *= mountReduction;

        int stage = Weight.GetStage();
        if (stage >= WeightStage.DamageReduction)
        {
            if (stage < WeightStage.SoftImmobile)
                _finalKnockbackResistance = float.Lerp(0f, 0.6f, Weight.GetClampedFactor(Weight.FromStage(WeightStage.DamageReduction), Weight.SoftImmobile));
            else
                _finalKnockbackResistance = 1f;
        }
        else
            _finalKnockbackResistance = 0f;

        if (stage < WeightStage.HardImmobile)
        {
            float basePenalty;
            if (stage < WeightStage.SoftImmobile)
            {
                float immobility = Weight.ClampedImmobility;
                basePenalty = float.Lerp(0f, 0.7f, immobility * immobility);
            }
            else
                basePenalty = 1f;
            _finalMovementFactor = Math.Clamp(1f - MovementPenalty.ApplyTo(basePenalty), 0f, 1f);
        }
        else
            _finalMovementFactor = Player.mount.Active ? 1f - mountReduction : 0f; // TODO: This sucks

        Player.runAcceleration *= _finalMovementFactor;
        Player.maxRunSpeed *= _finalMovementFactor;
        Player.accRunSpeed *= _finalMovementFactor;
        Player.jumpSpeed *= float.Lerp(0.2f, 1f, _finalMovementFactor);
    }

    public override void PostUpdateMiscEffects()
    {
        Vector2 acc = Player.velocity - _prevVel;
        _prevVel = Player.velocity;
        _squishRest = 1f;

        int stage = Weight.GetStage();
        ResizeHitbox(stage);

        // Weight loss
        if (!Player.mount.Active)
        {
            float factor = MathF.Abs(Player.velocity.X) * 0.5f;
            factor += MathF.Abs(acc.X) * 5f;
            if (JustJumped)
                factor += 5f;
            factor /= 60f * 60f; // ticks to minutes
            AddWeight(-MovementWeightLossRate.ApplyTo(factor));
        }

        // Ice break
        if (stage >= WeightStage.Heavy)
        {
            const int iceBreakTime = 60;
            if (Player.velocity.Y > -0.01f && HasIceBelow())
            {
                if (_iceBreakTimer == iceBreakTime / 2)
                    SoundEngine.PlaySound(SoundID.Item127);
                _iceBreakTimer++;
                if (_iceBreakTimer > iceBreakTime)
                    ThinIceBreak();
            }
            else
                _iceBreakTimer = 0;
        }
    }

    void ResizeHitbox(int stage)
    {
        // None of our business
        if ((Player.width + 12) % 16 != 0 || Player.height != Player.defaultHeight)
        {
            if (Player.mount.Active && Player.width != Player.defaultWidth) // However... vanilla mounts don't change the width. Cater to them.
            {
                float targetX = Player.position.X + Player.width * 0.5f - Player.defaultWidth * 0.5f;
                Player.width = Player.defaultWidth;
                Player.position.X = targetX;
            }
            return;
        }

        int targetWidth = Player.defaultWidth;
        if (!WgServerConfig.Instance.DisableFatHitbox && !Player.mount.Active && !Player.isLockedToATile)
            targetWidth = WeightValues.GetHitboxWidthInTiles(stage) * 16 - 12;
        if (Player.width != targetWidth)
        {
            float targetX = Player.position.X + Player.width * 0.5f - targetWidth * 0.5f;
            // Make sure we have enough space... otherwise we'd be able to walk through walls
            if (!Collision.SolidCollision(new Vector2(targetX, Player.position.Y), targetWidth, Player.height))
            {
                Player.width = targetWidth;
                Player.position.X = targetX;
            }
            else
                _squishRest = 1.2f;
        }
    }

    public override void PreUpdate()
    {
        PreUpdateVisuals();
    }

    public override void PostUpdate()
    {
        if (OwnsPlayer())
        {
            if (Stomach > 0f)
            {
                if (_digestTimer < 0)
                {
                    float delta = Stomach - MathF.Max(Stomach - Main.rand.NextFloat(DigestAmount * 0.5f, DigestAmount), 0f);
                    SetStomach(Stomach - delta);
                    AddWeight(delta);
                    if (Main.rand.NextBool(75))
                        Gurgle(true);
                    _digestTimer = Main.rand.Next(DigestTime, DigestTime * 2);
                }
                else
                    _digestTimer--;
            }
            else
                _digestTimer = DigestTime * 2;
        }

        UpdateAnimation();
        UpdateJiggle();
        PostUpdateVisuals();

        _finalMaxStage = Math.Clamp(MaxStage, 0, WeightStage.Max);
        if (_ignoreWgBuffTimer > 0)
            _ignoreWgBuffTimer--;

        int stage = Weight.GetStage();
        if (Player.sleeping.isSleeping && stage >= WeightStage.Obese)
        {
            Player.fullRotation = 0;
            Player.gfxOffY -= 16;
        }

        if (stage >= WeightStage.Fat)
            TownNPCRespawnSystem.unlockMilkmaid = true;
    }

    public override void UpdateDead()
    {
        _ignoreWgBuffTimer = 2;
    }

    // Taken from CheckIceBreak() in Player.cs
    void ThinIceBreak()
    {
        Vector2 pos = Player.position + Player.velocity;
        int xStart = (int)(pos.X / 16.0);
        int xEnd = (int)(((double)pos.X + Player.width) / 16.0);
        int yStart = (int)(((double)Player.position.Y + Player.height + 1.0) / 16.0);
        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yStart + 1 && Main.tile[x, y] != null; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasUnactuatedTile && tile.TileType == TileID.BreakableIce && !WorldGen.SolidTile(x, y - 1))
                {
                    WorldGen.KillTile(x, y);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.TileManipulation, number2: x, number3: y);
                }
            }
        }
    }

    // Not exactly proud of this...
    bool HasIceBelow()
    {
        Vector2 pos = Player.position + Player.velocity;
        int xStart = (int)(pos.X / 16.0);
        int xEnd = (int)(((double)pos.X + Player.width) / 16.0);
        int yStart = (int)(((double)Player.position.Y + Player.height + 1.0) / 16.0);
        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yStart + 2; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.HasUnactuatedTile && tile.TileType == TileID.BreakableIce && !WorldGen.SolidTile(x, y - 1))
                    return true;
            }
        }
        return false;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        SetWeight(new Weight(Weight.Mass * WeightValues.GetDeathPenalty(Player.difficulty)));
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        modifiers.Knockback *= 1f - _finalKnockbackResistance;
    }

    public override void ResetInfoAccessories()
    {
        _displayWeight = false;
    }

    public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
    {
        if (otherPlayer.Wg()._displayWeight)
            _displayWeight = true;
    }
}
