using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Summon;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.the_trueterrafox)]
public class CryoJelly : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 26;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out CryoJellyPlayer cj))
            return;

        cj.Active = true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }
}

public class CryoJellyPlayer : ModPlayer
{
    public bool Active;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Active || Player != Main.LocalPlayer || hit.DamageType != DamageClass.SummonMeleeSpeed)
            return;

        target.AddBuff(BuffID.Frostburn, Main.rand.Next(1, 4) * 2 * 60);
    }

    public override void EmitEnchantmentVisualsAt(Projectile projectile, Vector2 boxPosition, int boxWidth, int boxHeight)
    {
        if (!Active || Player != Main.LocalPlayer || projectile.DamageType != DamageClass.SummonMeleeSpeed)
            return;

        Player player = Main.LocalPlayer;

        float distance = projectile.position.Distance(player.position);

        if (distance < 100f)
            return;

        if (Main.rand.NextBool(3))
        {
            Dust dust = Dust.NewDustDirect(boxPosition, boxWidth, boxHeight, DustID.IceTorch, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 3f));
            dust.noGravity = true;
        }
    }
}
