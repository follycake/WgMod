using Terraria;
using Terraria.GameContent.Events;
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
public class NurseDialogNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.Nurse;
    }

    public static string GetText(string suffix, params object[] args)
    {
        return Language.GetTextValue("Mods.WgMod.Dialogue.Nurse." + suffix, args);
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

        int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
        int goblinTinkerer = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
        int mechanic = NPC.FindFirstNPC(NPCID.Mechanic);

        if (stage >= WeightStage.Chubby)
            chat.Add(GetText("PlayerStage" + stage));
        if (stage >= WeightStage.Fat)
            chat.Add(GetText("PlayerFatPlus"));
        if (stage >= WeightStage.Obese)
        {
            if (partyGirl >= 0)
                chat.Add(GetText("PlayerObesePlusPartyGirl"));
            if (goblinTinkerer >= 0 || mechanic >= 0)
                chat.Add(GetText("PlayerObesePlusGoblinMechanic"));
        }
        if (stage >= WeightStage.BarelyMobile)
        {
            if (partyGirl >= 0)
                chat.Add(GetText("PlayerBarelyMobilePlusPartyGirl", GetName(partyGirl)));
            if (Main.bloodMoon)
                chat.Add(GetText("PlayerBarelyMobilePlusBlood"));
        }
        if (stage >= WeightStage.Encumbered && BirthdayParty.PartyIsUp)
            chat.Add(GetText("PlayerEncumberedPlusParty"));

        int zoologist = NPC.FindFirstNPC(NPCID.BestiaryGirl);
        int dryad = NPC.FindFirstNPC(NPCID.Dryad);
        int groundedHarpy = NPC.FindFirstNPC(ModContent.NPCType<GroundedHarpyNPC>());
        int overflowingMimic = NPC.FindFirstNPC(ModContent.NPCType<OverflowingMimicNPC>());
        int milkmaid = NPC.FindFirstNPC(ModContent.NPCType<MilkmaidNPC>());

        if (zoologist >= 0 && WgGlobalNPC.GetStage(zoologist) > 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "ZoologistFatPlayerFat" : "ZoologistFat", GetName(zoologist)));
        if (dryad >= 0 && WgGlobalNPC.GetStage(dryad) > 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "DryadFatPlayerFat" : "DryadFat", GetName(dryad)));
        if (groundedHarpy >= 0 || overflowingMimic >= 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "MonstersFatPlayerFat" : "MonstersFat"));
        if (milkmaid >= 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "MilkmaidPlayerFat" : "Milkmaid", GetName(milkmaid)));

        finalChat = chat;
    }

    // folly: I don't feel like doing heal dialog right now
    /*public override void OnChatButtonClicked(NPC npc, bool firstButton)
    {
        if (!firstButton)
            return;
        WeightedRandom<string> chat = new();
        chat.Add(Main.npcChatText);

        Main.npcChatText = chat;
    }*/
}
