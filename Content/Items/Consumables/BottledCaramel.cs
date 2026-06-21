using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Consumables;

namespace WgMod.Content.Items.Consumables;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class BottledCaramel : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 20;

        ItemID.Sets.DrinkParticleColors[Type] =
        [
            new Color(240, 240, 240),
            new Color(200, 200, 200),
            new Color(140, 140, 140),
        ];
    }

    public static LocalizedText RestoreLifeText { get; private set; }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 26;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(silver: 15);

        Item.buffType = ModContent.BuffType<Caramel>();
        Item.buffTime = 30 * 60;

        Item.healLife = 120;
        Item.potion = true;
    }

    public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
    {
        healValue = Item.healLife;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.BottledHoney, 2).AddTile(TileID.Hellforge).Register();
    }
}
