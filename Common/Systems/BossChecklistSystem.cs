using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using WgMod.Common.GlobalNPCs;
using WgMod.Content.Items.Armor.Vanity;
using WgMod.Content.Items.Weapons.Melee;
using WgMod.Content.Items.Placeable.Furniture;
using WgMod.Content.Items.Pets;
using WgMod.Content.NPCs.UndergroundDesert.GorgeistBoss;
using Terraria.ID;

namespace WgMod.Common.Systems;

public class BossChecklistSystem : ModSystem
{
    public override void PostSetupContent()
    {
        DoBossChecklistIntegration();
    }

    public void DoBossChecklistIntegration()
    {
        if (!ModLoader.TryGetMod("BossChecklist", out Mod bcm) || bcm.Version < new Version(1, 6))
        {
            return;
        }

        List<int> gorgeistCollectibles =
            [
                ModContent.ItemType<GorgeistRelic>(),
                ModContent.ItemType<GorgeistHeart>(),
                ModContent.ItemType<GorgeistTrophy>(),
                ModContent.ItemType<GorgeistMask>()
            ];

        bcm.Call(
            "LogBoss",
            Mod,
            "Gorgeist",
            1.9f,
            DownedBossSystem.downedGorgeist,
            ModContent.NPCType<Gorgeist>(),
            new Dictionary<string, object>()
            {
                ["spawnItems"] = ModContent.ItemType<GorgeistHeart>(),
                ["collectibles"] = gorgeistCollectibles,
            }
        );
    }
}