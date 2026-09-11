using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Summon;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.the_trueterrafox)]
public class DamnedDessert : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 4);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out HellsDessertPlayer hd))
            return;

        hd.Active = true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MagmaStone)
            .AddIngredient<CryoJelly>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

public class HellsDessertPlayer : ModPlayer
{
    public bool Active;

    public override void ResetEffects()
    {
        Active = false;
    }

    readonly HashSet<DamageClass> _damageTypes =
    [
        DamageClass.Melee,
        DamageClass.SummonMeleeSpeed
    ];

    readonly HashSet<int> _debuffs =
    [
        BuffID.Frostburn2,
        BuffID.OnFire3,
    ];

    readonly HashSet<int> _dusts =
    [
        DustID.Torch,
        DustID.IceTorch
    ];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Active || Player != Main.LocalPlayer || !_damageTypes.Contains(hit.DamageType))
            return;

        foreach (var item in _debuffs)
            target.AddBuff(item, Main.rand.Next(1, 4) * 2 * 60);
    }

    public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
    {
        if (!Active || Player != Main.LocalPlayer || !_damageTypes.Contains(projectile.DamageType))
            return;

        Player player = Main.LocalPlayer;

        if (projectile.DamageType == DamageClass.SummonMeleeSpeed)
        {
            float distance = projectile.position.Distance(player.position);

            if (distance < 100f)
                return;
        }

        foreach (var item in _dusts)
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, item, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3f));
                dust.noGravity = true;
            }
    }

    public override void MeleeEffects(Item item, Rectangle hitbox)
    {
        if (!Active || Player != Main.LocalPlayer || !_damageTypes.Contains(item.DamageType))
            return;

        foreach (var item2 in _dusts)
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, item2, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3f));
                dust.noGravity = true;
            }
    }
}
