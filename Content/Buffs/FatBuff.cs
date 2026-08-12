using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Configs;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

public class FatBuff : WgBuffBase
{
    public const float MaxLifeIncreasePercentage = 0.2f;
    public const int MaxStageGraphic = WeightStage.Immobile;

    WgStat _damageReduction = new(0f, 0.05f);
    WgStat _lifeIncrease = new(0f, 100f);

    Asset<Texture2D> _stagesTexture;

    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
    }

    public override void Load()
    {
        if (Main.dedServ)
            return;
        _stagesTexture = ModContent.Request<Texture2D>($"{Texture}_Stages");
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
            return;
        int stage = wg.Weight.GetStage();
        buffName = this.GetLocalizedValue("Stages.Name" + stage);
        if (WgServerConfig.Instance.DisableFatBuffs)
        {
            tip = this.GetLocalizedValue("DisabledBuffs");
            return;
        }
        tip = base.Description.Format(
            (1f - wg._finalMovementFactor).Percent(),
            _damageReduction.Percent(),
            _lifeIncrease,
            wg._finalKnockbackResistance.Percent()
        );
        if (!WgServerConfig.Instance.DisableFatHitbox)
        {
            string line = this.GetLocalization("HitboxIncrease").Format((WeightValues.GetHitboxWidthInTiles(stage) - 2).Range(0, WeightValues.GetHitboxWidthInTiles(WeightStage.Max) - 2));
            tip += "\n" + line;
        }
        if (stage >= WeightStage.Blob)
            tip += "\n" + this.GetLocalization("CantMoveArms");
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (WgServerConfig.Instance.DisableFatBuffs || !player.TryGetModPlayer(out WgPlayer wg))
        {
            _damageReduction.Reset();
            _lifeIncrease.Reset();
            return;
        }

        // Calculate factors
        int stage = wg.Weight.GetStage();
        if (stage >= WeightStage.DamageReduction)
            _damageReduction.Lerp(wg.Weight.GetClampedFactor(Weight.FromStage(WeightStage.DamageReduction), Weight.Immobile));
        else
            _damageReduction.Reset();

        if (stage >= WeightStage.Heavy)
        {
            float t = wg.Weight.GetClampedFactor(Weight.FromStage(WeightStage.Heavy), Weight.Immobile) * MaxLifeIncreasePercentage;
            _lifeIncrease.Value = MathF.Floor(player.statLifeMax * t / 5f) * 5f;
            _lifeIncrease.Clamp();
        }
        else
            _lifeIncrease.Reset();

        // Apply factors
        player.endurance += _damageReduction;
        player.statLifeMax2 += _lifeIncrease;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        if (Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
        {
            drawParams.Texture = _stagesTexture.Value;
            drawParams.SourceRectangle = drawParams.Texture.Frame(1, MaxStageGraphic + 1, 0, Math.Clamp(wg.Weight.GetStage(), 0, MaxStageGraphic));
        }
        return base.PreDraw(spriteBatch, buffIndex, ref drawParams);
    }

    public override float GetProgress(WgPlayer wg, int buffIndex)
    {
        int stage = wg.Weight.GetStage();
        if (stage < WeightStage.Max)
            return wg.Weight.GetStageFactor();
        return 1f;
    }

    public override bool RightClick(int buffIndex)
    {
        return false;
    }
}
