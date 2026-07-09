using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WgMod.Common.Players;
using WgMod.Content.Buffs;

namespace WgMod.Content.Items.Armor.Vanity.Minty;

public static class MintySet
{
    public const ulong DevId = 76561198109349437uL;
    public static readonly Condition IsDev = new("Mods.WgMod.Conditions.MintyDev", () =>
    {
        try
        {
            return SteamUser.GetSteamID().m_SteamID == DevId;
        }
        catch
        {
            // Not on Steam?
        }
        return false;
    });
}

[Credit(ProjectRole.Programmer, Contributor.minty0985)]
[Credit(ProjectRole.Artist, Contributor.minty0985)]
[AutoloadEquip(EquipType.Head)]
public class MintyEars : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        //Item.value = Item.buyPrice(gold: 5);
        Item.vanity = true;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel)
            .AddCondition(MintySet.IsDev)
            .Register();
    }
}

[Credit(ProjectRole.Programmer, Contributor.minty0985)]
[Credit(ProjectRole.Artist, Contributor.minty0985)]
[AutoloadEquip(EquipType.Back)]
public class MintyTail : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        Item.accessory = true;
        Item.vanity = false;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;
        wg.FoodAbsorption *= 4f;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel)
            .AddCondition(MintySet.IsDev)
            .Register();
    }
}
