using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Dusts;

namespace WgMod.Content.Items.Armor.AmazonGarb;

[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
[AutoloadEquip(EquipType.Head)]
public class AmazonHeaddress : ModItem
{

    WgStat _damage = new(0.05f, 0.1f);
    WgStat _setBonusDamage = new(0.09f, 0.22f);
    WgStat _setBonusCritChance = new(18f, 36f);

    public static int HeadSlot { get; private set; }
    public static Asset<Texture2D> LavaTexture { get; set; }

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;

        HeadSlot = Item.headSlot;
        SetBonusText = this.GetLocalization("SetBonus");
    }

    public override void Load()
    {
        LavaTexture = ModContent.Request<Texture2D>(Texture + "_Head_Lava");
    }

    public static LocalizedText SetBonusText { get; private set; }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 10;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Orange;
        Item.defense = 1;
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _damage.Lerp(immobility);

        player.GetDamage(DamageClass.Melee) += _damage;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<AmazonToga>() && legs.type == ModContent.ItemType<AmazonSandals>();
    }

    public override void UpdateArmorSet(Player player)
    {
        if (!player.TryGetModPlayer(out AmazonGarbPlayer ag) || !player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.ClampedImmobility;

        _setBonusDamage.Lerp(immobility);
        _setBonusCritChance.Lerp(immobility);

        player.GetDamage(DamageClass.Ranged) += _setBonusDamage;
        player.GetCritChance(DamageClass.Ranged) += _setBonusCritChance;

        ag._active = true;

        if (ag._cooldown < 15 * 60)
        {
            ag._cooldown++;

            if (ag._cooldown == 15 * 60)
            {
                SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.5f });

                Dust.NewDustPerfect(player.Top, ModContent.DustType<CutieHeart>(), new Vector2(0, -1), 1, default, 1);
            }
        }

        player.setBonus = SetBonusText.Format();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_damage.Percent());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 6)
            .AddIngredient(ItemID.Silk, 12)
            .Register();
    }
}

public class AmazonGarbPlayer : ModPlayer
{
    public const int CooldownMax = 15 * 60;
    public bool _active;
    public int _cooldown = CooldownMax;
    //public int _visual;

    public override void ResetEffects()
    {
        _active = false;
    }

    public override bool ConsumableDodge(Player.HurtInfo info)
    {
        if (info.Damage < Player.statLifeMax && _active && _cooldown == CooldownMax)
        {
            amazonDodge();
            return true;
        }

        return false;
    }

    public void amazonDodge()
    {
        Player.SetImmuneTimeForAllTypes(Player.longInvince ? 120 : 80);

        for (int i = 0; i < 50; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
            Dust d = Dust.NewDustPerfect(Player.Center + speed * 16, ModContent.DustType<CutieHeart>(), speed * 5, Scale: 1.5f);
            d.noGravity = true;
        }

        SoundEngine.PlaySound(SoundID.Shatter with { Pitch = 0.5f });

        if (Player.whoAmI != Main.myPlayer)
            return;

        _cooldown = 0;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (!_active)
            return;

        if (proj.DamageType == DamageClass.Ranged || proj.DamageType == DamageClass.Throwing)
            proj.DamageType = DamageClass.Melee;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (Player.head == AmazonHeaddress.HeadSlot)
        {
            if (Player.TryGetModPlayer(out WgPlayer wg))
                wg._headOverride = AmazonHeaddress.LavaTexture;
        }
    }

    /*public override void PostUpdateEquips()
    {
        _visual = Math.Clamp(_visual + (_active ? 1 : -1), 0, 30);
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (_visual > 0)
            g = Math.Max(0, g - _visual * 0.03f);
    }*/
}
