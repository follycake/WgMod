using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class BottomlessAppetite : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 22;

        Item.accessory = true;
        Item.rare = ItemRarityID.Red;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out BottomlessAppetitePlayer ba))
            return;
        float immobility = wg.Weight.ClampedImmobility;
        ba._active = true;
        ba._range = (int)float.Lerp(2f, 9999f, immobility);
        ba._hidden = hideVisual;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar, 6)
            .AddIngredient(ItemID.FragmentNebula, 6)
            .AddIngredient(ItemID.FragmentVortex, 6)
            .AddIngredient(ItemID.FragmentStardust, 6)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out BottomlessAppetitePlayer bp))
            return;

        bp._active = true;

        if (player.whoAmI == Main.myPlayer && !player.dead)
        {
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (
                    npc.active
                    && !npc.friendly
                    && npc.damage > 0
                    && !npc.dontTakeDamage
                    && !npc.buffImmune[ModContent.BuffType<PillarWrath>()]
                    && player.CanNPCBeHitByPlayerOrPlayerProjectile(npc)
                    && Vector2.Distance(player.Center, npc.Center) <= bp._radius + 32f
                )
                    npc.AddBuff(ModContent.BuffType<PillarWrath>(), 2 * 60);
            }
        }
    }
}

public class BottomlessAppetitePlayer : ModPlayer
{
    internal bool _active;
    internal bool _hidden;
    internal int _range;
    public float _radius;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!Player.TryGetModPlayer(out WgPlayer wg) || drawInfo.shadow != 0f || Player.dead)
            return;
        float immobility = wg.Weight.ClampedImmobility;

        int dustRate = 1;
        float dustIntensity = float.Lerp(3f, 5f, immobility);
        float dustSize = float.Lerp(1f, 1.5f, immobility);

        float playerX = Player.Center.X;
        float playerY = Player.Center.Y;
        float playerWidth = Player.width;
        float playerHeight = Player.height;

        if (playerWidth > playerHeight)
            _radius = playerWidth * 1.5f;
        else
            _radius = playerHeight * 1.5f;

        if (_active == true && _hidden == false)
        {
            for (int i = 0; i < dustRate; i++)
            {
                float angle = (float)(Main.rand.NextDouble() * Math.Tau);
                Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));
                const float spin = 0.75f;
                Vector2 dir2 = new(MathF.Cos(angle + spin), MathF.Sin(angle + spin));

                int dust = Dust.NewDust(
                    new Vector2(playerX + dir.X * _radius, playerY + dir.Y * _radius),
                    4,
                    4,
                    DustID.t_Slime,
                    dir2.X * -dustIntensity + Player.velocity.X,
                    dir2.Y * -dustIntensity + Player.velocity.Y,
                    (int)(dustIntensity * 20),
                    new Color(0, 0, 0),
                    dustSize
                );
                int dust2 = Dust.NewDust(
                    new Vector2(playerX + dir.X * _radius, playerY + dir.Y * _radius),
                    4,
                    4,
                    DustID.SolarFlare,
                    0f,
                    0f,
                    (int)(dustIntensity * 20),
                    default,
                    dustSize
                );
                int dust3 = Dust.NewDust(
                    new Vector2(playerX + dir.X * _radius, playerY + dir.Y * _radius),
                    4,
                    4,
                    DustID.SolarFlare,
                    -dir2.X * -dustIntensity + Player.velocity.X,
                    -dir2.Y * -dustIntensity + Player.velocity.Y,
                    (int)(dustIntensity * 20),
                    default,
                    dustSize
                );

                Main.dust[dust].noGravity = true;
                Main.dust[dust2].noGravity = true;
                Main.dust[dust3].noGravity = true;
                Main.dust[dust2].velocity = Player.velocity;
            }
        }
    }
}

public class BottomlessAppetiteItem : GlobalItem
{
    public override void GrabRange(Item item, Player player, ref int grabRange)
    {
        if (!player.TryGetModPlayer(out BottomlessAppetitePlayer ba))
            return;
        if (ba._active && item.type != ItemID.FallenStar)
            grabRange *= ba._range;
    }
}
