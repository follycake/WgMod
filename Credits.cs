using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.UI.Chat;
using WgMod.Common.Configs;

namespace WgMod;

public enum ProjectRole
{
    Programmer,
    Artist,
    VFX,
    SFX,
    Idea
}

// Discord handles
public enum Contributor
{
    [Category("Team")]
    follycake,
    maimaichubs,
    jumpsu2,
    _d_u_m_m_y_,
    igobee_,
    minty0985,

    [Category("Contributors")]
    ubulumn,
    trilophyte,
    sinnerdrip,
    subparnitragen,
    haydumbb,
    divine_lumine,
    the_trueterrafox,

    [Category("Other")]
    purple_circle
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CreditAttribute(ProjectRole role, Contributor contributor) : Attribute
{
    public ProjectRole Role { get; } = role;
    public Contributor Contributor { get; } = contributor;
}

public static class Credits
{
    public static TextSnippet AutoCredits { get; private set; }
    public static readonly Dictionary<Type, TooltipLine> Tooltips = [];

    public static short GetIcon(ProjectRole role) => role switch
    {
        ProjectRole.Programmer => ItemID.ArtisanLoaf,
        ProjectRole.Artist => ItemID.PaintRoller,
        ProjectRole.VFX => ItemID.HallowBossDye,
        ProjectRole.SFX => ItemID.Megaphone,
        ProjectRole.Idea => ItemID.Candle,
        _ => ItemID.None
    };

    public static void Scan(Mod mod)
    {
        Dictionary<Contributor, List<string>> roles = [];
        foreach (Contributor contributor in Enum.GetValues<Contributor>())
            roles.Add(contributor, [$"@{contributor}"]);

        Type[] types = AssemblyManager.GetLoadableTypes(mod.Code);
        foreach (Type type in types)
        {
            object[] credits = type.GetCustomAttributes(typeof(CreditAttribute), false);
            if (credits.Length <= 0)
                continue;
            string text = "";
            for (int i = 0; i < credits.Length; i++)
            {
                CreditAttribute credit = (CreditAttribute)credits[i];
                if (i != 0)
                    text += "\n";
                text += $"[i:{GetIcon(credit.Role)}] [c/808080:{credit.Role}: @{credit.Contributor}]";
                roles[credit.Contributor].Add($"- {type.Name} ({credit.Role})");
            }
            Tooltips.Add(type, new TooltipLine(mod, "Credits", text));
        }

        StringBuilder builder = new();
        foreach (var list in roles.Values)
        {
            if (list.Count <= 1)
                continue;
            foreach (string item in list)
                builder.AppendLine(item);
            builder.AppendLine();
        }
        AutoCredits = new TextSnippet(builder.ToString());
        ChatManager.Register<CreditsTagHandler>("wgmodcred");
    }
}

public class CreditsGlobalItem : GlobalItem
{
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (!WgClientConfig.Instance.ShowCredits)
            return;
        if (item.ModItem is not ModItem modItem || modItem.Mod != Mod)
            return;
        if (Credits.Tooltips.TryGetValue(modItem.GetType(), out TooltipLine line))
            tooltips.Add(line);
    }
}

public class CreditsGlobalBuff : GlobalBuff
{
    public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
    {
        base.ModifyBuffText(type, ref buffName, ref tip, ref rare);
        if (!WgClientConfig.Instance.ShowCredits)
            return;
        if (ModContent.GetModBuff(type) is not ModBuff buff || buff.Mod != Mod)
            return;
        if (Credits.Tooltips.TryGetValue(buff.GetType(), out TooltipLine line))
            tip += "\n" + line.Text;
    }
}

public class CreditsTagHandler : ITagHandler
{
    public TextSnippet Parse(string text, Color baseColor = default, string options = null)
    {
        return Credits.AutoCredits;
    }
}
