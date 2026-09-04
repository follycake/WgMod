using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;

namespace WgMod.Content.Items.Armor.WhaleArmor;

[AutoloadEquip(EquipType.Body)]
[Credit(ProjectRole.Programmer, Contributor.maimaichubs)]
[Credit(ProjectRole.Artist, Contributor.divine_lumine)]
public class WhaleBody : ModItem
{
    public static int BodySlot { get; private set; }
    public static int TailSlot { get; private set; }

    WgStat _damage = new(0.06f, 0.12f);
    WgStat _health = new(50f, 100f);
    WgStat _fishing = new(5f, 15f);

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 14;
        Item.value = Item.sellPrice(gold: 2, silver: 40);
        Item.rare = ItemRarityID.LightRed;
        Item.defense = 20;
    }

    public override void SetStaticDefaults()
    {
        BodySlot = Item.bodySlot;
    }

    public override void Load()
    {
        if (Main.dedServ)
            return;
        TailSlot = EquipLoader.AddEquipTexture(Mod, Texture + "_Tail", EquipType.Back, this, nameof(WhaleBody) + "_Tail");
    }

    public override void UpdateEquip(Player player)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        float immobility = wg.Weight.GetClampedFactor(WeightStage.Regular, WeightStage.Blob);

        _damage.Lerp(immobility);
        _health.Lerp(immobility);
        _fishing.Lerp(immobility);

        _health.Value = MathF.Floor(_health.Value / 5f) * 5f;

        player.GetDamage(DamageClass.Generic) += _damage;
        player.statLifeMax2 += _health;
        player.fishingSkill += _fishing;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.Torso;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.AnglerVest)
            .AddIngredient(ItemID.AdamantiteBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.AnglerVest)
            .AddIngredient(ItemID.TitaniumBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        tooltips.FormatLines(_health, _fishing, _damage.Percent());
    }
}

public class WhaleBodyPlayer : ModPlayer
{
    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        if (Player.body == WhaleBody.BodySlot)
            Player.back = WhaleBody.TailSlot;
    }
}
