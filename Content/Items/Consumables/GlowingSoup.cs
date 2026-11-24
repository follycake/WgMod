using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.Buffs.Debuffs;

namespace WgMod.Content.Items.Consumables;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GlowingSoup : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 5;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemID.Sets.FoodParticleColors[Type] =
        [
            new Color(0,0,255),
            new Color(75,85,255),
            new Color(150,160,255)
        ];

        ItemID.Sets.IsFood[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToFood(
            22,
            22,
            BuffID.WellFed,
            Utility.TimeToTicks(minutes: 18)
        );
        Item.value = Item.sellPrice(silver: 50);
        Item.rare = ItemRarityID.Blue;
    }

    public override bool? UseItem(Player player)
    {
        player.AddBuff(ModContent.BuffType<Spored>(), Utility.TimeToTicks(minutes: 2));
        return true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Food;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => false;
    public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
    {
        Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);

        Vector2 drawOrigin = itemFrame.Size() / 2f;
        Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);

        spriteBatch.Draw(TextureAssets.Item[Item.type].Value, drawPosition, itemFrame, lightColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);

        string path = Texture;
        Asset<Texture2D> asset = ModContent.Request<Texture2D>(path + "_Glowmask");

        spriteBatch.Draw(asset.Value, drawPosition, itemFrame, Color.White, rotation, drawOrigin, scale, SpriteEffects.None, 0);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GlowingMushroom, 2)
            .AddIngredient(ItemID.GlowingSnail)
            .AddTile(TileID.CookingPots)
            .Register();
        CreateRecipe()
            .AddIngredient(ItemID.GlowingMushroom, 2)
            .AddIngredient(ItemID.TruffleWorm)
            .AddTile(TileID.CookingPots)
            .Register();
    }
}
