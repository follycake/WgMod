using System.Collections.Generic;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Tombstones;

public abstract class FatTombstoneItem : ModItem
{
    public static readonly Dictionary<int, int> StyleToItem = [];

    public abstract int Style { get; }

    public override void SetStaticDefaults()
    {
        StyleToItem[Style] = Type;
    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FatTombstones>(), Style);
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 15;
        Item.ResearchUnlockCount = 2;
    }
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class StatueGrave : FatTombstoneItem
{
    public override int Style => 0;
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class PieGrave : FatTombstoneItem
{
    public override int Style => 1;
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class TurkeyGrave : FatTombstoneItem
{
    public override int Style => 2;
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class BellyGrave : FatTombstoneItem
{
    public override int Style => 3;
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class ButtGrave : FatTombstoneItem
{
    public override int Style => 4;
}

[Credit(ProjectRole.Artist, Contributor._d_u_m_m_y_)]
public class CakeGrave : FatTombstoneItem
{
    public override int Style => 5;
}
