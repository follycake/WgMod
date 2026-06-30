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

namespace WgMod.Common.Players;

public partial class WgPlayer : ModPlayer
{
    /// <summary> The player's weight </summary>
    public Weight Weight { get; private set; } = Weight.Base;

    /// <summary> How much movement will be reduced because of the player's weight, multiply this </summary>
    public StatModifier MovementPenalty;

    /// <summary> How fast the player will lose weight due to movement, add or subtract to this </summary>
    public StatModifier WeightLossRate;

    /// <summary> How much weight the player will gain due to food, multiply this </summary>
    public StatModifier FoodAbsorption;

    /// <summary> The maximum weight stage that the player can reach </summary>
    public int MaxStage;

    public readonly int[] BuffDuration = new int[Player.MaxBuffs];
    internal int _ignoreWgBuffTimer = 2;

    internal float _finalKnockbackResistance;
    internal float _finalMovementFactor = 1f;
    internal int _finalMaxStage = Weight.ImmobileStage;

    internal float _buffTotalGain;
    internal int _iceBreakTimer;
    internal bool _displayWeight;

    Vector2 _prevVel;

    public override void Initialize()
    {
        SetWeightForced(Weight.Base, false);
        InitializeVisuals();
    }

    public override void OnEnterWorld()
    {
        _ignoreWgBuffTimer = 2;
    }

    public bool CanSetWeight()
    {
        return !Main.dedServ && Player.whoAmI == Main.myPlayer;
    }

    public void SetWeight(Weight weight, bool effects = true)
    {
        if (!CanSetWeight())
            return;
        if (WgClientConfig.Instance.DisableWeightGain)
            weight = new Weight(Math.Min(weight.Mass, Weight.Mass));
        SetWeightForced(weight, effects);
    }

    void SetWeightForced(Weight weight, bool effects = true)
    {
        int prevStage = Weight.GetStage();
        Weight = Weight.Clamp(weight, _finalMaxStage);
        if (Weight.GetStage() != prevStage && effects)
        {
            SoundEngine.PlaySound(WgSounds.Belly, Player.Center);
            _squishPos += 0.06f;
        }
    }

    public override void ResetEffects()
    {
        // Custom stats
        MovementPenalty = StatModifier.Default;
        WeightLossRate = StatModifier.Default;
        FoodAbsorption = StatModifier.Default;
        MaxStage = Weight.ImmobileStage;
    }

    public override void PreUpdateBuffs()
    {
        // Ensure fat buff
        int type = ModContent.BuffType<FatBuff>();
        if (!Player.HasBuff(type))
            Player.AddBuff(type, 60);
    }

    public override void PostUpdateRunSpeeds()
    {
        if (WgServerConfig.Instance.DisableFatBuffs)
            return;

        if (Player.mount.Active)
            MovementPenalty *= 0.8f;

        int stage = Weight.GetStage();
        if (stage >= Weight.DamageReductionStage)
        {
            if (stage < Weight.ImmobileStage)
                _finalKnockbackResistance = float.Lerp(0f, 0.6f, Weight.GetClampedFactor(Weight.FromStage(Weight.DamageReductionStage), Weight.Immobile));
            else
                _finalKnockbackResistance = 1f;
        }
        else
            _finalKnockbackResistance = 0f;

        float basePenalty;
        if (stage < Weight.ImmobileStage)
        {
            float immobility = Weight.ClampedImmobility;
            basePenalty = float.Lerp(0f, 0.7f, immobility * immobility);
        }
        else
            basePenalty = 1f;
        _finalMovementFactor = Math.Clamp(1f - MovementPenalty.ApplyTo(basePenalty), 0f, 1f);

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
            float factor = MathF.Abs(Player.velocity.X);
            factor += MathF.Abs(acc.X) * 20f;
            factor *= 0.0002f;
            SetWeight(Weight - WeightLossRate.ApplyTo(factor));
        }

        // Ice break
        if (stage >= Weight.HeavyStage)
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
            return;

        int targetWidth = Player.defaultWidth;
        if (!WgServerConfig.Instance.DisableFatHitbox && !Player.mount.Active && !Player.isLockedToATile)
            targetWidth = WeightValues.GetHitboxWidthInTiles(stage) * 16 - 12;
        if (Player.width != targetWidth)
        {
            float centerX = Player.position.X + Player.width * 0.5f;
            float targetX = centerX - targetWidth * 0.5f;
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
        UpdateJiggle();
        PostUpdateVisuals();

        _finalMaxStage = Math.Clamp(MaxStage, 0, Weight.MaxStage);
        if (_ignoreWgBuffTimer > 0)
            _ignoreWgBuffTimer--;

        int stage = Weight.GetStage();
        if (Player.sleeping.isSleeping && Weight.GetStage() >= 4)
        {
            Player.fullRotation = 0;
            Player.gfxOffY -= 16;
        }

        if (stage >= 3)
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
