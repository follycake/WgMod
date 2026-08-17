using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class GlutGut : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 30;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.buyPrice(gold: 6);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HallowedBar, 8)
            .AddIngredient(ItemID.SoulofMight, 12)
            .AddIngredient(ItemID.Wire, 6)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out GlutGutPlayer gp))
            return;

        gp._active = true;
    }
}

public class GlutGutPlayer : ModPlayer
{
    internal bool _active;

    public override void ResetEffects()
    {
        _active = false;
    }
}

public class GlutGutItem : GlobalItem
{
    public override bool ConsumeItem(Item item, Player player)
    {
        if (!player.TryGetModPlayer(out GlutGutPlayer gp))
            return base.ConsumeItem(item, player);

        if (gp._active)
        {
            switch (item.useStyle)
            {
                case ItemUseStyleID.DrinkOld:
                case ItemUseStyleID.EatFood:
                case ItemUseStyleID.DrinkLiquid:
                case ItemUseStyleID.DrinkLong:
                    player.AddBuff(ModContent.BuffType<GluttedGut>(), 30 * 60);
                    SoundEngine.PlaySound(SoundID.Item33);
                    break;
            }
        }

        return base.ConsumeItem(item, player);
    }
}
