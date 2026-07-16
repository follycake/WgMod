using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;


namespace WgMod.Content.Items.Accessories.Magic;

[AutoloadEquip(EquipType.Wings)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
[Credit(ProjectRole.VFX, Contributor._d_u_m_m_y_)]
public class LiftingTome : ModItem
{
    WgStat _magicDamage = new(0.06f, 0.12f);
    WgStat _manaCost = new(0.96f, 0.92f);
    WgStat _maxMana = new(20, 60);
    WgStat _flight = new(1f, 1.5f);
    public string _texture;

    public override void SetStaticDefaults()
    {
        ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(40, 5f, 1f);
    }

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.value = Item.buyPrice(gold: 4);
        Item.rare = ItemRarityID.Orange;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out LiftingTomePlayer lt))
            return;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _magicDamage.Lerp(immobility);
        _manaCost.Lerp(immobility);
        _maxMana.Lerp(immobility);
        _maxMana.Value = MathF.Floor(_maxMana.Value / 20f) * 20f;

        player.GetDamage(DamageClass.Magic) += _magicDamage;
        player.manaCost *= _manaCost;
        player.statManaMax2 += _maxMana;

        lt._active = true;

        if (lt._cooldownBolt < lt._cooldownBoltMax)
            lt._cooldownBolt++;

        if (lt._cooldownSkull < lt._cooldownSkullMax)
            lt._cooldownSkull++;

        if (lt._cooldownScythe < lt._cooldownScytheMax)
            lt._cooldownScythe++;

        lt.SpawnHallucination(Item);
    }

    public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _flight.Lerp(immobility);

        speed += _flight * 3;
        acceleration *= _flight;
    }

    public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _flight.Lerp(immobility);

        ascentWhenFalling *= _flight;
        ascentWhenRising *= _flight;
        maxCanAscendMultiplier *= _flight;
        maxAscentMultiplier *= _flight;
        constantAscend *= _flight;
    }

    public override void AddRecipes()
    {
        if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
        {
            CreateRecipe()
                .AddIngredient(ItemID.BoneHelm)
                .AddIngredient(ItemID.WaterBolt)
                .AddIngredient(ItemID.BookofSkulls)
                .AddIngredient(ItemID.DemonScythe)
                .AddIngredient(calamity.Find<ModItem>("PurifiedGel").Type, 5)
                .AddTile(TileID.DemonAltar)
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
                .Register();
        }
        else
        {
            CreateRecipe()
                .AddIngredient(ItemID.BoneHelm)
                .AddIngredient(ItemID.WaterBolt)
                .AddIngredient(ItemID.BookofSkulls)
                .AddIngredient(ItemID.DemonScythe)
                .AddTile(TileID.DemonAltar)
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1))
                .Register();
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(
            (1 - _manaCost).Percent(),
            _maxMana,
            _magicDamage.Percent()
        );
    }
}

public class LiftingTomePlayer : ModPlayer
{
    public bool _active;
    public int _cooldownBolt;
    public int _cooldownSkull;
    public int _cooldownScythe;
    public int _cooldownBoltMax = 120;
    public int _cooldownSkullMax = 180;
    public int _cooldownScytheMax = 240;

    public WgStat _damageModifier = new(1f, 1.5f);
    public WgStat _velocityModifier = new(1f, 0.8f);

    public override void ResetEffects()
    {
        _active = false;
    }

    public void SpawnHallucination(Item item)
    {
        if (Player.whoAmI != Main.myPlayer)
            return;
        Player.insanityShadowCooldown = Utils.Clamp(Player.insanityShadowCooldown - 1, 0, 100);
        if (Player.insanityShadowCooldown > 0)
            return;
        Player.insanityShadowCooldown = Main.rand.Next(20, 101);
        float num = 500f;
        int damage = 18;
        _hallucinationCandidates.Clear();
        for (int index = 0; index < 200; ++index)
        {
            NPC npc = Main.npc[index];
            if (
                npc.CanBeChasedBy(this)
                && (double)Player.Distance(npc.Center) <= (double)num
                && Collision.CanHitLine(
                    Player.position,
                    Player.width,
                    Player.height,
                    npc.position,
                    npc.width,
                    npc.height
                )
            )
                _hallucinationCandidates.Add(npc);
        }
        if (_hallucinationCandidates.Count == 0)
            return;
        Projectile.RandomizeInsanityShadowFor(
            Main.rand.NextFromCollection(_hallucinationCandidates),
            false,
            out Vector2 spawnposition,
            out Vector2 spawnvelocity,
            out float ai0,
            out float ai1
        );
        Projectile.NewProjectile(
            new EntitySource_ItemUse(Player, item),
            spawnposition,
            spawnvelocity,
            ProjectileID.InsanityShadowFriendly,
            damage,
            0.0f,
            Player.whoAmI,
            ai0,
            ai1
        );
    }

    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        if (_active && Player.velocity.Y == 0f)
            PlayerDrawLayers.Wings.Hide();
    }

    static readonly List<NPC> _hallucinationCandidates = [];
}

public class LiftingTomeWingLayer : PlayerDrawLayer
{
    static readonly Vector2[] _offsets =
    [
        new Vector2(6, 0),
        new Vector2(-6, 0),
        new Vector2(0, 6),
        new Vector2(0, -6)
    ];

    static int _equipSlot;

    public override void SetStaticDefaults()
    {
        _equipSlot = EquipLoader.GetEquipSlot(Mod, nameof(LiftingTome), EquipType.Wings);
    }

    public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        if (!PlayerDrawLayers.Wings.Visible)
            return;

        Player player = drawInfo.drawPlayer;
        if (player.wings != _equipSlot)
            return;

        Asset<Texture2D> wingTexture = TextureAssets.Wings[_equipSlot];
        Rectangle sourceRect = wingTexture.Frame(1, 4);
        Vector2 drawPos = drawInfo.Position + new Vector2(player.width / 2f - 9f * player.direction, player.height / 2f + player.gfxOffY) - Main.screenPosition;

        Vector2 origin = sourceRect.Size() / 2f;
        SpriteEffects effects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        Color baseColor = new(112, 33, 117, 50); // Base purple colour
        foreach (Vector2 offset in _offsets)
        {
            Vector2 pos = drawPos + offset;
            drawInfo.DrawDataCache.Add(new DrawData(
                wingTexture.Value,
                pos,
                sourceRect,
                baseColor,
                0f,
                origin,
                1f,
                effects,
                0
            ));
        }

        // Tints the middle hand black
        drawInfo.DrawDataCache.Add(new DrawData(
            wingTexture.Value,
            drawPos,
            sourceRect,
            new Color(68, 61, 69, 255),
            0f,
            origin,
            1f,
            effects,
            0
        ));
    }
}

public class LiftingTomeItem : GlobalItem
{
    public override void UseAnimation(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out LiftingTomePlayer lt) || !player.TryGetModPlayer(out WgPlayer wg) || !lt._active || item.damage < 1 || player.whoAmI != Main.myPlayer)
            return;

        float immobility = wg.Weight.ClampedImmobility;

        Vector2 mousePosition = Main.MouseWorld;
        float angle = Utils.AngleTo(player.Center, mousePosition);
        Vector2 velocity = new(MathF.Cos(angle), MathF.Sin(angle));

        lt._damageModifier.Lerp(immobility);
        lt._velocityModifier.Lerp(immobility);

        switch (item.useStyle)
        {
            case ItemUseStyleID.Swing:
            case ItemUseStyleID.Thrust:
            case ItemUseStyleID.Shoot:
            case ItemUseStyleID.Guitar:
            case ItemUseStyleID.Rapier:
            case ItemUseStyleID.RaiseLamp:

                if (lt._cooldownScythe == lt._cooldownScytheMax)
                {
                    lt._cooldownScythe = (int)float.Lerp(30, 0, immobility);

                    Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        player.Center,
                        velocity * 0.2f * lt._velocityModifier,
                        ProjectileID.DemonScythe,
                        17 * lt._damageModifier,
                        5
                    );

                    return;
                }

                if (lt._cooldownSkull == lt._cooldownSkullMax)
                {
                    lt._cooldownSkull = (int)float.Lerp(30, 0, immobility);

                    Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        player.Center,
                        velocity * 3.5f * lt._velocityModifier,
                        ProjectileID.BookOfSkullsSkull,
                        14 * lt._damageModifier,
                        3.5f
                    );

                    return;
                }


                if (lt._cooldownBolt == lt._cooldownBoltMax)
                {
                    lt._cooldownBolt = (int)float.Lerp(30, 0, immobility);

                    Projectile.NewProjectile(
                        player.GetSource_FromThis(),
                        player.Center,
                        velocity * 4.5f * lt._velocityModifier,
                        ProjectileID.WaterBolt,
                        9 * lt._damageModifier,
                        5
                    );
                }

                break;
        }
    }
}
