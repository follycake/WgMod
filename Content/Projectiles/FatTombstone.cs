using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Items.Placeable.Tombstones;
using WgMod.Content.Tiles;

namespace WgMod.Content.Projectiles;

public class FatTombstone : ModProjectile
{
    public override string Texture => "WgMod/Assets/Textures/Invisible";

    public int Style => (int)Projectile.ai[2];

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Tombstone);
    }

    public override void Load()
    {
        On_Player.DropTombstone += DropTombstone;
        On_Projectile.VanillaAI += VanillaAI;
    }

    public override void Unload()
    {
        On_Player.DropTombstone -= DropTombstone;
        On_Projectile.VanillaAI -= VanillaAI;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Item[FatTombstoneItem.StyleToItem[Style]].Value;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, 1f, SpriteEffects.None);
        return false;
    }

    static void DropTombstone(On_Player.orig_DropTombstone orig, Player self, long coinsOwned, NetworkText deathText, int hitDirection)
    {
        if (!self.TryGetModPlayer(out WgPlayer wg) || wg.Weight.GetStage() <= WeightStage.Regular || !Main.rand.NextBool())
        {
            orig(self, coinsOwned, deathText, hitDirection);
            return;
        }
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        float num;
        for (num = Main.rand.Next(-35, 36) * 0.1f; num < 2f && num > -2f; num += Main.rand.Next(-30, 31) * 0.1f)
        {
        }
        int type = Main.rand.Next(6);
        if (coinsOwned <= 100000)
            type = (type != 0) ? (200 + type) : 43;
        else
        {
            type = Main.rand.Next(5);
            type += 527;
        }
        IEntitySource projectileSource_Misc = self.GetSource_Misc("PlayerDeath_TombStone");
        int damage = 0;
        int knockback = 0;
        if (Main.getGoodWorld)
        {
            damage = 70;
            knockback = 10;
        }
        int ai0 = self.whoAmI;
        int style = FatTombstones.GetStyle(type);
        type = ModContent.ProjectileType<FatTombstone>();
        int proj;
        if (!Main.getGoodWorld)
            proj = Projectile.NewProjectile(projectileSource_Misc, self.position.X + self.width / 2, self.position.Y + self.height / 2, Main.rand.Next(10, 30) * 0.1f * hitDirection + num, Main.rand.Next(-40, -20) * 0.1f, type, damage, knockback, Main.myPlayer, ai0, 0f, style);
        else
            proj = Projectile.NewProjectile(projectileSource_Misc, self.position.X + self.width / 2, self.position.Y + self.height / 2, (Main.rand.Next(10, 30) * 0.1f * hitDirection + num) * 1.5f, Main.rand.Next(-40, -20) * 0.1f * 1.5f, type, damage, knockback, Main.myPlayer, ai0, 0f, style);
        DateTime now = DateTime.Now;
        string text = now.ToString("D");
        if (GameCulture.FromCultureName(GameCulture.CultureName.English).IsActive)
            text = now.ToString("MMMM d, yyy");
        string miscText = deathText.ToString() + "\n" + text;
        Main.projectile[proj].miscText = miscText;
    }

    static void VanillaAI(On_Projectile.orig_VanillaAI orig, Projectile self)
    {
        if (self.type != ModContent.ProjectileType<FatTombstone>())
        {
            orig(self);
            return;
        }
        if (self.velocity.Y == 0f)
            self.velocity.X *= 0.98f;
        self.rotation += self.velocity.X * 0.1f;
        self.velocity.Y += 0.2f;
        if (Main.getGoodWorld && Math.Abs(self.velocity.X) + Math.Abs(self.velocity.Y) < 1f)
        {
            self.damage = 0;
            self.knockBack = 0f;
        }
        if (self.owner != Main.myPlayer)
            return;
        int placeX = (int)((self.position.X + self.width / 2) / 16f);
        int placeY = (int)((self.position.Y + self.height - 4f) / 16f);
        if (Main.tile[placeX, placeY] == null)
            return;
        int style = (int)self.ai[2];
        bool placed = false;
        if (TileObject.CanPlace(placeX, placeY, ModContent.TileType<FatTombstones>(), style, self.direction, out TileObject objectData))
            placed = TileObject.Place(objectData);
        if (placed)
        {
            NetMessage.SendObjectPlacement(-1, placeX, placeY, objectData.type, objectData.style, objectData.alternate, objectData.random, self.direction);
            SoundEngine.PlaySound(SoundID.Dig, new Vector2(placeX * 16, placeY * 16));
            int sign = Sign.ReadSign(placeX, placeY);
            if (sign >= 0)
            {
                Sign.TextSign(sign, self.miscText);
                NetMessage.SendData(MessageID.ReadSign, -1, -1, null, sign, 0f, (byte)new BitsByte(b1: true));
            }
            self.Kill();
        }
    }
}
