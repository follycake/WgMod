using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Curses;

public class CurseTemplate : ModItem
{
    public virtual int CurseID => -1;

    public override bool IsLoadingEnabled(Mod mod) => CurseID >= 0;

    public override LocalizedText DisplayName => Language.GetText("Mods.WgMod.Items.Curses." + base.Name + ".DisplayName");

    public override LocalizedText Tooltip => Language.GetText("Mods.WgMod.Items.Curses." + base.Name + ".Tooltip");

    public override string Texture => "WgMod/Assets/Placeholder/ExampleItem";

    public override void SetDefaults()
    {
        Item.rare = ItemRarityID.LightPurple;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.UseSound = SoundID.Item103;
        Item.width = 16;
        Item.height = 16;
    }

    public override bool? UseItem(Player player)
    {
        if (CurseID < 0)
            return true;

        if (player.TryGetModPlayer(out CursePlayer plr))
        {
            plr.ActiveCurses[CurseID] = !plr.ActiveCurses[CurseID];
            return true;
        }

        return null;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        if (CurseID == -1)
            return true;
        if (Main.LocalPlayer.TryGetModPlayer(out CursePlayer plr))
        {
            if (plr.ActiveCurses[CurseID])
            {
                string path = Texture;
                if (path == "WgMod/Assets/Placeholder/ExampleItem")
                    return true;
                Asset<Texture2D> asset = ModContent.Request<Texture2D>(path + "_Active");
                spriteBatch.Draw(asset.Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0);
                return false;
            }
        }
        return true;

    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.LineBeforeTooltip(out TooltipLine line);
        if (Main.LocalPlayer.TryGetModPlayer(out CursePlayer plr))
            if (plr.ActiveCurses[CurseID])
                tooltips.Insert(tooltips.IndexOf(line) + 1, new TooltipLine(Mod, "NewTooltip", Language.GetTextValue("Mods.WgMod.Items.Curses.General.Active")));
            else
                tooltips.Insert(tooltips.IndexOf(line) + 1, new TooltipLine(Mod, "NewTooltip", Language.GetTextValue("Mods.WgMod.Items.Curses.General.Inactive")));
    }


    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.RemainingUseItems;
    }
}

public enum CurseIDs
{
    Stuckage,
    WeakChains,
    WeakPulley,
    Absorbant,
    ThirteenDollarStaminaSystem,
    RockBottom,
    SlimeEater,
    MoreFatteningEnemies,
    StormDrain,
    SuperSoft
}
