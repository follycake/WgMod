using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.trilophyte)]
[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class FlaskOfAmbrosia : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 36;

        Item.accessory = true;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 4);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        if (!player.TryGetModPlayer(out AmbrosiaPlayer ap))
            return;
        wg.MovementWeightLossRate += 2f;
        ap._active = true;
        ap._hidden = hideVisual;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SweetheartNecklace)
            .AddIngredient<AmuletOfStarving>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}

public class AmbrosiaPlayer : ModPlayer
{
    internal bool _active;
    internal bool _hidden;
    internal int _dustRate;

    public override void ResetEffects()
    {
        _active = false;
        _hidden = false;
    }

    public override void DrawEffects(
        PlayerDrawSet drawInfo,
        ref float r,
        ref float g,
        ref float b,
        ref float a,
        ref bool fullBright
    )
    {
        _dustRate = 30;

        if (Main.rand.NextBool(_dustRate) && _active == true && _hidden == false)
            Dust.NewDust(
                Player.position,
                Player.width,
                Player.height - 1,
                DustID.YellowTorch,
                0f,
                0f,
                100,
                default,
                0.7f
            );
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (_active)
        {
            Player.AddBuff(ModContent.BuffType<AmbrosiaGorged>(), 8 * 60);
            SoundEngine.PlaySound(WgSounds.Gulp, Player.Center);

            for (int i = 0; i < 50; i++)
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.t_Honey,
                    0f,
                    0.5f,
                    100,
                    default,
                    1.3f
                );
        }
    }
}
