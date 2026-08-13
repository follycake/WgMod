
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.NPCs.UndergroundDesert;

namespace WgMod.Content.Projectiles.Enemy.Gorgeist;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class TossedFood : ModProjectile
{
    static readonly int[] _items =
    [
        ItemID.ChristmasPudding,
        ItemID.GingerbreadCookie,
        ItemID.RoastedBird,
        ItemID.MonsterLasagna,
        ItemID.BananaSplit,
        ItemID.Fries,
        ItemID.Burger,
        ItemID.Pizza,
        ItemID.IceCream,
        ItemID.Hotdog,
        ItemID.Milkshake
    ];

    int _itemIndex;
    int _itemId;

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = true;

        Projectile.height = 24;
        Projectile.width = 24;

        _itemIndex = 0;
        _itemId = _items[_itemIndex];
    }


    public override void OnSpawn(IEntitySource source)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        _itemIndex = Main.rand.Next(_items.Length);
        _itemId = _items[_itemIndex];
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] >= 15f)
        {
            Projectile.ai[0] = 15f;
            Projectile.velocity.Y += 0.1f;
        }

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[1] == 1)
            NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<HomingFood>(), default, 1);
    }
}
