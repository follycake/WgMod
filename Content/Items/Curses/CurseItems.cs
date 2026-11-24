using Terraria.ID;
using WgMod.Common.Systems;

namespace WgMod.Content.Items.Curses;

[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class Stuckage : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.Stuckage;
    public override string Texture => "WgMod/Content/Items/Curses/Stuckage";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.WoodenDoor)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class WeakChains : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.WeakChains;
    public override string Texture => "WgMod/Content/Items/Curses/WeakChains";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Chain, 3)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class WeakPulley : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.WeakPulley;
    public override string Texture => "WgMod/Content/Items/Curses/WeakPulley";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.RopeCoil)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class Absorbant : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.Absorbant;
    public override string Texture => "WgMod/Content/Items/Curses/Absorbant";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.EmptyBucket)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class ThirteenDollarStaminaSystem : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.ThirteenDollarStaminaSystem;
    public override string Texture => "WgMod/Content/Items/Curses/Exhaustion";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class RockBottom : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.RockBottom;
    public override string Texture => "WgMod/Content/Items/Curses/RockBottom";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Boulder)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class SlimeEater : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.SlimeEater;
    public override string Texture => "WgMod/Content/Items/Curses/SlimeEater";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Gel, 25)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class MoreFatteningEnemies : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.MoreFatteningEnemies;
    public override string Texture => "WgMod/Content/Items/Curses/AlluringScent";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Sunflower)
            .AddIngredient(ItemID.BottledHoney)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class StormDrain : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.StormDrain;
    public override string Texture => "WgMod/Content/Items/Curses/StormDrain";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Grate, 3)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
[Credit(ProjectRole.Programmer, Contributor.jumpsu2)]
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class SuperSoft : CurseTemplate
{
    public override int CurseID => (int)CurseIDs.SuperSoft;
    public override string Texture => "WgMod/Content/Items/Curses/SuperSoft";
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Pumpkin, 5)
            .AddRecipeGroup(RecipeGroups.EvilPowder, 5)
            .AddTile<Tiles.Furniture.AltarOfCurses>()
            .Register();
    }
}
