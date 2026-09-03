using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Fat;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
public class StarlightBoots : ModItem
{
    public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";

    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;

        Item.accessory = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(silver: 75);
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out StarlightBootsPlayer sl))
            return;
        sl._enabled = true;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Accessories;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        Player me = Main.LocalPlayer;
        if (!me.TryGetModPlayer(out StarlightBootsPlayer sl))
            return;
        // this is stupid
        tooltips.FormatLines(sl._speed.Percent(), sl._acceleration.Percent());
    }
}

public class StarlightBootsPlayer : ModPlayer
{
    internal bool _enabled;
    internal WgStat _acceleration = new(0.25f, 0.75f);
    internal WgStat _speed = new(0.1f, 1f);

    public override void ResetEffects()
    {
        _enabled = false;
    }

    public override void PostUpdateRunSpeeds()
    {
        if (!_enabled)
            return;

        float lerping = Player.Wg().Weight.GetClampedFactor(Weight.Base, Weight.FromStage(WeightStage.Blob));

        _acceleration.Lerp(lerping);
        _speed.Lerp(lerping);

        if (Player.Grounded())
        {
            Player.runAcceleration *= 1f + _acceleration;
            Player.runSlowdown *= 1f + _acceleration;
        }

        Player.maxRunSpeed *= 1f + _speed;
        Player.accRunSpeed *= 1f + _speed;
    }

    public override void PostUpdateMiscEffects()
    {
        if (!_enabled)
            return;

        bool running = false;
        float runningSpeed = 0f;
        if (Player.Grounded() && !Player.mount.Active)
        {
            if ((Player.controlRight && Player.velocity.X > 0) || (Player.controlLeft && Player.velocity.X < 0))
            {
                running = true;
                runningSpeed = Player.velocity.X > 0 ? Player.velocity.X : Player.velocity.X * -1;
            }
        }
        if (running)
        {
            Mass weightGain = 0.003f * runningSpeed;
            Player.Wg().AddWeight(weightGain);
        }
    }
}

