using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Buffs;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class Stamina : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }
    public override bool RightClick(int buffIndex)
    {
        return false;
    }

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        Player player = Main.LocalPlayer;

        float val = float.Lerp(2f, 0.6f, player.Curses()._stamina / player.Curses()._maxStamina);

        if (val > 1)
        {
            buffName = Language.GetTextValue("Mods.WgMod.Buffs.Stamina.AltDisplayName");
            string percentage = (val - 1).Percent(false);
            tip = Language.GetText("Mods.WgMod.Buffs.Stamina.AltDescription").Format(percentage);
        }
        else
        {
            string percentage = (1 - val).Percent(false);
            tip = Language.GetText("Mods.WgMod.Buffs.Stamina.Description").Format(percentage);
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
    {
        Player player = Main.LocalPlayer;

        float val = float.Lerp(2f, 0.6f, player.Curses()._stamina / player.Curses()._maxStamina);

        drawParams.SourceRectangle = drawParams.Texture.Frame(2, 1, val > 1 ? 1 : 0);
        return base.PreDraw(spriteBatch, buffIndex, ref drawParams);
    }
}
