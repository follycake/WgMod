using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Accessories.Summon;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
public class HellRaiser : ModItem
{
    public const int MinionCount = 3;

    WgStat _minionDamage = new(-0.1f, 0.1f);
    WgStat _whipSpeed = new(0.9f, 0.8f);

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.buyPrice(gold: 1);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        float immobility = wg.Weight.ClampedImmobility;
        _minionDamage.Lerp(immobility);
        _whipSpeed.Lerp(immobility);

        player.maxMinions += MinionCount;
        player.GetDamage(DamageClass.Summon) += _minionDamage;
        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) *= _whipSpeed;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 12)
            .AddIngredient(ItemID.SoulofNight, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(MinionCount, _minionDamage.Percent(), (1f - _whipSpeed).Percent());
    }
}
