
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Content.NPCs.UndergroundDesert;

namespace WgMod.Content.Projectiles.Enemy.Gorgeist;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
public class TossedFood : ModProjectile
{
    public const int Death = 3 * 30;

    public ref float Timer => ref Projectile.localAI[0];
    public ref float DeathTimer => ref Projectile.localAI[1];
    public bool TransformOnDeath => Projectile.ai[0] > 0f;

    public int ItemIndex
    {
        get => (int)Projectile.ai[1] - 1;
        set => Projectile.ai[1] = value + 1;
    }

    public int ItemId => HomingFood.Items[ItemIndex];

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = true;

        Projectile.height = 24;
        Projectile.width = 24;

        ItemIndex = -1;
        Timer = 0f;
        DeathTimer = 0f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (ItemIndex < 0)
            ItemIndex = Main.rand.Next(HomingFood.Items.Length);
    }

    public override void AI()
    {
        Timer++;
        if (Timer >= 15f)
        {
            Timer = 15f;
            Projectile.velocity.Y += 0.1f;
        }
        if (Projectile.velocity.Y > 16f)
            Projectile.velocity.Y = 16f;
        if (DeathTimer < Death)
            DeathTimer++;
        else
            Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        for (int i = 0; i < 5; i++)
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
        if (TransformOnDeath)
            NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<HomingFood>(), ai3: Projectile.ai[1]);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int item = ItemId;
        Main.instance.LoadItem(item);
        Asset<Texture2D> texture = TextureAssets.Item[item];
        Rectangle frame = texture.Frame(1, 3);
        Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);
        return false;
    }
}
