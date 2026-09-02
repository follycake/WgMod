using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.DungeonStatues;

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkBellyOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>());
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkBreastOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>(), 1);
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkButtOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentButtStatueTile>());
        Item.width = 44;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueBellyOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>(), 2);
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueBreastOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>(), 3);
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueButtOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentButtStatueTile>(), 1);
        Item.width = 44;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenBellyOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>(), 4);
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenBreastOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentStatueTile>(), 5);
        Item.width = 36;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenButtOverindulgentStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.OverindulgentButtStatueTile>(), 2);
        Item.width = 44;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 15)
            .AddIngredient(ItemID.Ectoplasm, 1)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
