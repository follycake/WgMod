using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Melee;

namespace WgMod.Common.GlobalItems;

public class ItemDisabling : GlobalItem
{
    /// <summary> Whether or not an accessory in the Band of Sweetening line is currently equipped. </summary>
    public static bool BandLine;
    /// <summary> The name of the accessory in the Band of Sweetening line with the highest equip order </summary>
    public static string BandName;

    /// <summary> Every accessory in the Band of Sweetening line </summary>
    static readonly HashSet<int> _bandLine = [
        ItemID.BandofRegeneration,
        ItemID.PhilosophersStone,
        ItemID.CharmofMyths,
        ModContent.ItemType<BandOfSweetening>(),
        ModContent.ItemType<CharmOfSweets>(),
    ];

    /// <summary> Whether or not an accessory in the Queenly Gluttony is currently equipped. </summary>
    public static bool GauntletLine;
    /// <summary> The name of the accessory in the Queenly Gluttony line with the highest equip order </summary>
    public static string GauntletName;

    /// <summary> Every accessory in the Queenly gluttony line </summary>
    static readonly HashSet<int> _gauntletLine = [
        ModContent.ItemType<QueenlyGluttony>(),
        ModContent.ItemType<SolDrive>(),
    ];

    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (player != Main.LocalPlayer)
            return;

        // Band of Sweetening line

        if (_bandLine.Contains(item.type))
        {
            if (!BandLine)
                BandName = item.Name;

            BandLine = true;
        }

        // Queenly Gluttony Line

        if (_gauntletLine.Contains(item.type))
        {
            if (!GauntletLine)
                GauntletName = item.Name;

            GauntletLine = true;
        }
    }

    public override void UpdateInventory(Item item, Player player)
    {
        BandLine = false;
        GauntletLine = false;
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (_bandLine.Contains(item.type))
            BandTooltipFucker(item, tooltips);

        if (_gauntletLine.Contains(item.type))
            GauntletTooltipFucker(item, tooltips);
    }

    /// <summary> Adds an additional tooltip line to accessories in the Band of Sweetening line to portray them being disabled </summary>
    public void BandTooltipFucker(Item item, List<TooltipLine> tooltips)
    {
        if (!BandLine || BandName == item.Name)
            return;

        tooltips.LineBeforeTooltip(out TooltipLine line);
        tooltips.Insert(tooltips.IndexOf(line) + 1, new TooltipLine(Mod, "NewTooltip", Language.GetTextValue("Mods.WgMod.GlobalItem.Disabled", BandName)));
    }

    /// <summary> Adds an additional tooltip line to accessories in the Queenly Gluttony line to portray them being disabled </summary>
    public void GauntletTooltipFucker(Item item, List<TooltipLine> tooltips)
    {
        if (!GauntletLine || GauntletName == item.Name)
            return;

        tooltips.LineBeforeTooltip(out TooltipLine line);
        tooltips.Insert(tooltips.IndexOf(line) + 1, new TooltipLine(Mod, "NewTooltip", Language.GetTextValue("Mods.WgMod.GlobalItem.Disabled", GauntletName)));
    }
}
