using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using WgMod.Common.Players;
using WgMod.Content.NPCs.TownNPCs.GroundedHarpy;
using WgMod.Content.NPCs.TownNPCs.Milkmaid;
using WgMod.Content.NPCs.TownNPCs.OverflowingMimic;

namespace WgMod.Common.GlobalNPCs.Dialog;

[Credit(ProjectRole.Programmer, Contributor.follycake)]
[Credit(ProjectRole.Dialog, Contributor.ANONYMOUS)]
public class DryadDialogNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Dryad;
    }

    public static string GetText(string suffix, params object[] args)
    {
        return Language.GetTextValue("Mods.WgMod.Dialogue.Dryad." + suffix, args);
    }

    public static string GetName(int npc)
    {
        return Main.npc[npc].GivenName;
    }

    public override void GetChat(NPC npc, ref string finalChat)
    {
        Player player = Main.LocalPlayer;
        if (!player.TryGetModPlayer(out WgPlayer wg))
            return;

        int stage = wg.Weight.GetStage();
        WeightedRandom<string> chat = new();
        chat.Add(finalChat);

        if (stage >= WeightStage.Chubby)
            chat.Add(GetText("PlayerStage" + stage, player.Male ? "moobs" : "boobs"));
        if (stage >= WeightStage.Obese)
            chat.Add(GetText("PlayerObesePlus"));
        if (stage >= WeightStage.MorbidlyObese)
            chat.Add(GetText("PlayerMorbidlyObesePlus"));
        if (stage >= WeightStage.Encumbered)
            chat.Add(GetText("PlayerEncumberedPlus"));

        int nurse = NPC.FindFirstNPC(NPCID.Nurse);
        if (WgGlobalNPC.GetStage(npc) > 0)
        {
            chat.Add(GetText("Fat"));
            if (Main.raining)
                chat.Add(GetText(npc.homeless ? "FatRainingHomeless" : "FatRaining"));
            if (nurse >= 0)
            {
                chat.Add(GetText("FatNurse", GetName(nurse)));
                if (stage >= WeightStage.Fat)
                    chat.Add(GetText("FatPlayerFatNurse", GetName(nurse)));
            }
            if (player.ZoneJungle)
                chat.Add(GetText("FatJungle"));
            if (player.ZoneDesert)
                chat.Add(GetText("FatDesert"));
            if (stage >= WeightStage.Chubby)
                chat.Add(GetText("FatPlayerFat"));
            if (!wg.IsMobile)
                chat.Add(GetText("FatPlayerNotMobile"));
        }

        int overflowingMimic = NPC.FindFirstNPC(ModContent.NPCType<OverflowingMimicNPC>());
        int groundedHarpy = NPC.FindFirstNPC(ModContent.NPCType<GroundedHarpyNPC>());
        int milkmaid = NPC.FindFirstNPC(ModContent.NPCType<MilkmaidNPC>());
        int zoologist = NPC.FindFirstNPC(NPCID.BestiaryGirl);

        if (overflowingMimic >= 0)
            chat.Add(GetText("OverflowingMimic"));
        if (groundedHarpy >= 0)
            chat.Add(GetText("FattenedWildlife"));
        if (milkmaid >= 0)
            chat.Add(GetText("Milkmaid", GetName(milkmaid)));
        if (zoologist >= 0 && WgGlobalNPC.GetStage(zoologist) > 0)
            chat.Add(GetText("ZoologistFat", GetName(zoologist)));

        finalChat = chat;
    }
}
