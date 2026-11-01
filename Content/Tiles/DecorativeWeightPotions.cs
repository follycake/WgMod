using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WgMod.Content.Items.Consumables.Potions.WeightGainPotions;
using WgMod.Content.Items.Consumables.Potions.WeightLossPotions;

namespace WgMod.Content.Tiles;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
public class DecorativeWeightPotions : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        DustType = DustID.Glass;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.addTile(Type);

        AddMapEntry(new Color(133, 213, 247), Mod.GetLocalization("MapObject.DecorativeWeightPotions"));
    }

    public override bool KillSound(int i, int j, bool fail)
    {
        if (fail)
            return true;
        SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i * 16f, j * 16f));
        return false;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j)
    {
        Tile tile = Main.tile[i, j];
        int style = TileObjectData.GetTileStyle(tile);
        switch (style)
        {
            case 0:
                yield return new Item(ModContent.ItemType<WeightGainPotion>());
                break;
            case 1:
                yield return new Item(ModContent.ItemType<SuperWeightGainPotion>());
                break;
            case 2:
                yield return new Item(ModContent.ItemType<WeightLossPotion>());
                break;
        }
    }
}
