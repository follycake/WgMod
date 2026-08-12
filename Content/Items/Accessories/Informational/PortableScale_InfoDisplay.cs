using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Accessories.Informational;

public class PortableScale_InfoDisplay : InfoDisplay
{
    public override bool Active() => Main.LocalPlayer.Wg()._displayWeight;

    public override string DisplayValue(ref Color displayColor, ref Color displayShadowColor)
    {
        Weight weight = Main.LocalPlayer.Wg().Weight;
        return weight.ToString();
    }
}
