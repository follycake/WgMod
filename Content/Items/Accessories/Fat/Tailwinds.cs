using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.maimaichubs)]
public class Tailwinds : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;

        Item.expert = true;
        Item.accessory = true;
        Item.value = Item.buyPrice(gold: 2);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out TailwindsPlayer tp))
            return;
        tp._active = true;
        tp._hidden = hideVisual;
    }
}

public class TailwindsPlayer : ModPlayer
{
    public bool _active;
    public bool _hidden;

    public int _windDirection = 0;

    public override void ResetEffects()
    {
        _active = false;
        _hidden = false;
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!_active || Player != Main.LocalPlayer || _hidden)
            return;

        if (Main.windSpeedCurrent > 0)
            _windDirection = 1;
        else
            _windDirection = -1;

        if (_windDirection == Player.direction && Main.rand.NextBool(60))
            Dust.NewDust(Player.position, Player.width, Player.height, DustID.Cloud, Player.velocity.X + Main.windSpeedCurrent, Player.velocity.Y, 50);
    }
}

public class TailwindsItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public int _windDirection = 0;
    public int _projectileDirection = 0;

    public float _modifier;

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (!player.TryGetModPlayer(out TailwindsPlayer tp) || !tp._active)
            return;

        _modifier = 1 + (Main.windSpeedCurrent / 2.5f);

        int particles = (int)((_modifier - 1) * 10);

        //Main.NewText($"modifier: {_modifier}");
        //Main.NewText($"particles: {particles}");

        if (Main.windSpeedCurrent > 0)
            _windDirection = 1;
        else
            _windDirection = -1;

        if (velocity.X > 0)
            _projectileDirection = 1;
        else
            _projectileDirection = -1;

        if (_projectileDirection == _windDirection)
        {
            damage = (int)(damage * _modifier);
            velocity *= _modifier;

            for (int i = 0; i < particles; i++)
                Dust.NewDustDirect(position, 0, 0, DustID.Cloud, velocity.X, velocity.Y, 50);
        }
    }
}
