using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Summon;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.the_trueterrafox)]
public class CryoGauntlet : ModItem
{
    WgStat _damage = new(0.08f, 0.14f);
    WgStat _attackSpeed = new(0.08f, 0.14f);
    WgStat _knockback = new(1.8f, 2.2f);
    WgStat _size = new(0.06f, 0.12f);

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 26;

        Item.accessory = true;
        Item.rare = ItemRarityID.Lime;
        Item.value = Item.sellPrice(gold: 6);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg) || !player.TryGetModPlayer(out CryoJellyPlayer cj))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        DamageClass whips = DamageClass.SummonMeleeSpeed;

        _damage.Lerp(immobility);
        _attackSpeed.Lerp(immobility);
        _knockback.Lerp(immobility);
        _size.Lerp(immobility);

        player.GetDamage(whips) += _damage;
        player.GetAttackSpeed(whips) += _attackSpeed;
        player.GetKnockback(whips) += _knockback;
        player.whipRangeMultiplier += _size;

        cj.Active = true;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent(), _knockback, _size.Percent());
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MechanicalGlove)
            .AddIngredient<CryoJelly>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}
