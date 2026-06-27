using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Armor.Vanity;

public static class BlackRiderSet
{
    public const ulong DevId = 76561198984695415uL;
    public static readonly Condition IsDev = new("Mods.WgMod.Conditions.BlackRiderDev", () =>
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

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
[AutoloadEquip(EquipType.Head)]
public class BlackRiderHair : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        //Item.value = Item.buyPrice(gold: 5);
        Item.vanity = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel)
            .AddCondition(BlackRiderSet.IsDev)
            .Register();
    }
}

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
[AutoloadEquip(EquipType.Body)]
public class BlackRiderChest : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        //Item.value = Item.buyPrice(gold: 5);
        Item.vanity = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel)
            .AddCondition(BlackRiderSet.IsDev)
            .Register();
    }
}

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Artist, Contributor.igobee_)]
[AutoloadEquip(EquipType.Legs)]
public class BlackRiderSkirt : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.rare = ItemRarityID.Cyan;
        //Item.value = Item.buyPrice(gold: 5);
        Item.vanity = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel)
            .AddCondition(BlackRiderSet.IsDev)
            .Register();
    }
}
