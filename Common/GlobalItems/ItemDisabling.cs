using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Content.Items.Accessories.Fat;
using WgMod.Content.Items.Accessories.Melee;
using WgMod.Content.Items.Accessories.Movement.Boots;

namespace WgMod.Common.GlobalItems;

public class ItemDisabling : GlobalItem
{
    public class Line
    {
        public readonly HashSet<int> Items;

        /// <summary> The item that takes priority </summary>
        public Item ActiveItem;

        /// <summary> Whether the line is active or not </summary>
        public bool Active => ActiveItem != null;

        Line(HashSet<int> items)
        {
            Items = items;
        }

        public static Line Create(HashSet<int> items)
        {
            Line line = new(items);
            Lines.Add(line);
            return line;
        }

        public void Reset()
        {
            ActiveItem = null;
        }

        public void Update(Item item)
        {
            if (IsInLine(item.type))
            {
                if (!Active)
                    ActiveItem = item;
            }
        }

        public bool IsInLine(int item)
        {
            return Items.Contains(item);
        }

        public bool IsInLine(Item item)
        {
            return IsInLine(item.type);
        }
    };

    /// <summary> Every single accessory line </summary>
    public static readonly List<Line> Lines = [];

    /// <summary> Every accessory in the Band of Sweetening line </summary>
    public static readonly Line BandLine = Line.Create([
        ItemID.BandofRegeneration,
        ItemID.PhilosophersStone,
        ItemID.CharmofMyths,
        ModContent.ItemType<BandOfSweetening>(),
        ModContent.ItemType<CharmOfSweets>(),
    ]);

    /// <summary> Every accessory in the Queenly gluttony line </summary>
    public static readonly Line GauntletLine = Line.Create([
        ModContent.ItemType<QueenlyGluttony>(),
        ModContent.ItemType<SolDrive>(),
    ]);

    /// <summary> Every accessory in the Exoskeleton Legs line </summary>
    public static readonly Line BootsLine = Line.Create([
        ModContent.ItemType<ExoskeletonLegs>(),
        ModContent.ItemType<TerraskeletonLegs>(),
        ModContent.ItemType<MechaskeletonLegs>(),
        ModContent.ItemType<TwilightTracers>(),
    ]);

    public override void UpdateInventory(Item item, Player player)
    {
        foreach (Line line in Lines)
            line.Reset();
    }

    public override void UpdateAccessory(Item item, Player player, bool hideVisual)
    {
        if (player != Main.LocalPlayer)
            return;
        foreach (Line line in Lines)
            line.Update(item);
    }

    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        foreach (Line line in Lines)
        {
            if (line.Active && line.ActiveItem.type != item.type && line.IsInLine(item))
            {
                tooltips.LineBeforeTooltip(out TooltipLine tooltipLine);
                tooltips.Insert(tooltips.IndexOf(tooltipLine) + 1, new TooltipLine(Mod, "NewTooltip", Language.GetTextValue("Mods.WgMod.GlobalItem.Disabled", line.ActiveItem.Name)));
            }
        }
    }

    public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
    {
        foreach (Line line in Lines)
            if (line.IsInLine(equippedItem) && line.IsInLine(incomingItem))
                return false;

        return true;
    }
}
