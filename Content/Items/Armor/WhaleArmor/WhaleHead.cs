using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.WhaleArmor;

[AutoloadEquip(EquipType.Head)]

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class WhaleHead : ModItem
{
    const float SetBonusWeightLoss = 0.5f;

    const int TimerMax = 120;

    WgStat _crit = new(0.02f, 0.06f);
    WgStat _health = new(50f, 100f);
    WgStat _fishing = new(5f, 10f);

    WgStat _setBonusSpeed = new(1f, 0.5f);
    WgStat _setBonusJump = new(5f, 10f);
    WgStat _setBonusFallSpeedRate = new(0.1f, 0.2f);
    WgStat _setBonusWaterSpeed = new(2f, 10f);
    WgStat _blowholeDamage = new(200f, 500f);

    float _setBonusFallSpeed = 0f;

    int _timer = TimerMax;

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(gold: 3);
        Item.rare = ItemRarityID.LightRed;
        Item.defense = 20;
    }

    public override void SetStaticDefaults()
    {
        SetBonusText = this.GetLocalization("SetBonus");
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.GetClampedFactor(WeightStage.Regular, WeightStage.Blob);

        _crit.Lerp(immobility);
        _health.Lerp(immobility);
        _fishing.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.GetCritChance(DamageClass.Generic) += _crit;
        player.statLifeMax2 += _health;
        player.fishingSkill += _fishing;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<WhaleBody>()
            && legs.type == ModContent.ItemType<WhaleLegs>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.GetClampedFactor(WeightStage.Regular, WeightStage.Blob);

        _setBonusSpeed.Lerp(immobility);
        _setBonusJump.Lerp(immobility);
        _setBonusFallSpeedRate.Lerp(immobility);
        _setBonusWaterSpeed.Lerp(immobility);
        _blowholeDamage.Lerp(immobility);

        wg.MovementPenalty *= _setBonusSpeed;
        wg.PreventImmobility = true;
        wg.MovementWeightLossRate *= SetBonusWeightLoss;
        player.jumpSpeedBoost += _setBonusJump;

        player.breathEffectiveness += 3f;
        player.accFlipper = true;

        if (player.wet)
            player.moveSpeed += _setBonusWaterSpeed;
        else
            FallSpeedAccelerate(player, _setBonusFallSpeedRate);

        BreachAttack(player, _blowholeDamage);

        player.setBonus = SetBonusText.Format((1f - _setBonusSpeed).Percent(), (1f - SetBonusWeightLoss).Percent(), _blowholeDamage);
    }

    public void BreachAttack(Player player, int damage)
    {
        if (_timer < TimerMax)
            _timer++;

        if (_timer > 30 && _timer < TimerMax && _timer % 5 == 0)
        {
            Vector2 blowhole = new(player.position.X + (player.width / 2f), player.position.Y);

            SoundEngine.PlaySound(SoundID.Item13, blowhole);

            Projectile water = Projectile.NewProjectileDirect(player.GetSource_FromThis(), blowhole, new(Main.rand.NextFloat(-0.5f, 0.5f), -10 + Main.rand.NextFloat()), ProjectileID.WaterStream, (int)player.GetTotalDamage(DamageClass.Generic).ApplyTo(damage), 2f);

            water.DamageType = DamageClass.Generic;
        }

        if (player.wet)
            _timer = 0;
    }

    public void FallSpeedAccelerate(Player player, float rate)
    {
        if (!CheckForSolidGround(player))
        {
            if (player.maxFallSpeed < 15f)
                _setBonusFallSpeed += rate;
        }
        else
            _setBonusFallSpeed = 0f;

        player.maxFallSpeed += _setBonusFallSpeed;
    }

    static bool CheckForSolidGround(Player player)
    {
        List<Point> tiles = Collision.GetTilesIn(player.Hitbox.BottomLeft() - new Vector2(-2, -2), player.Hitbox.BottomRight() + new Vector2(2, 6));
        bool hasSolidTile = false;
        foreach (var point in tiles)
        {
            Tile tile = Framing.GetTileSafely(point);
            if (tile.HasTile)
            {
                if (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])
                    hasSolidTile = true;
            }
        }

        return hasSolidTile;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Headgear;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AnglerHat)
            .AddIngredient(ItemID.AdamantiteBar, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.AnglerHat)
            .AddIngredient(ItemID.TitaniumBar, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_health, _fishing, _crit.Percent());
    }
}
