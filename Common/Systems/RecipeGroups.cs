using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WgMod.Common.Systems;

public class RecipeGroups : ModSystem
{
    public static RecipeGroup EvilPowder;
    public override void Unload()
    {
        EvilPowder = null;
    }

    public override void AddRecipeGroups()
    {
        // Language.GetTextValue("LegacyMisc.37") is the word "Any" in english, and the corresponding word in other languages

        EvilPowder = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " " + Language.GetTextValue("Mods.WgMod.RecipeGroups.EvilPowder"),
            ItemID.VilePowder, ItemID.ViciousPowder);
        RecipeGroup.RegisterGroup("WgMod:EvilPowder", EvilPowder);
    }
}
