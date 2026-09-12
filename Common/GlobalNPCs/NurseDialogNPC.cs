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

namespace WgMod.Common.GlobalNPCs;

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

    public override void GetChat(NPC npc, ref string finalChat)
    {
        if (!Main.LocalPlayer.TryGetModPlayer(out WgPlayer wg))
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
                chat.Add(GetText("PlayerBarelyMobilePlusPartyGirl", Main.npc[partyGirl].GivenName));
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

        if (zoologist >= 0 && WgGlobalNPC.GetStage(Main.npc[zoologist]) > 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "ZoologistFatPlayerFat" : "ZoologistFat", Main.npc[zoologist].GivenName));
        if (dryad >= 0 && WgGlobalNPC.GetStage(Main.npc[dryad]) > 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "DryadFatPlayerFat" : "DryadFat", Main.npc[dryad].GivenName));
        if (groundedHarpy >= 0 || overflowingMimic >= 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "MonstersFatPlayerFat" : "MonstersFat"));
        if (milkmaid >= 0)
            chat.Add(GetText(stage >= WeightStage.Fat ? "MilkmaidPlayerFat" : "Milkmaid", Main.npc[milkmaid].GivenName));

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
