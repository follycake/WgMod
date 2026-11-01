using Terraria.ID;
using Terraria.ModLoader;

namespace WgMod.Content.Items.Placeable.Furniture.DungeonStatues;

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkBellyEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>());
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkBreastEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 1);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class PinkButtEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 2);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.PinkBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueBellyEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 3);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueBreastEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 4);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class BlueButtEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 5);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.BlueBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenBellyEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 6);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenBreastEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 7);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}

[Credit(ProjectRole.Artist, Contributor.jumpsu2)]
public class GreenButtEncumberedStatueItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PlacedEncumberedStatue>(), 8);
        Item.width = 32;
        Item.height = 36;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.PlacableObjects;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GreenBrick, 10)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
    }
}
