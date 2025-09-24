// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using JetBrains.Annotations;
using OhHey.Services;

namespace OhHey.UI;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class MainWindow : Window
{
    private readonly TargetService _targetService;
    private readonly EmoteService _emoteService;
    private readonly float _textWidth;

    public MainWindow(TargetService targetService, EmoteService emoteService) : base("Oh Hey!##ohhey_main_window")
    {
        _targetService = targetService;
        _emoteService = emoteService;
        _textWidth = ImGui.CalcTextSize("00:00:00 A name that is 20 char used Emote Name here").X;

        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(_textWidth + ImGui.GetStyle().WindowPadding.X * 2, 310)
        };
    }

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("##ohhey_main_tab_bar");
        if(ImGui.BeginTabItem("Targets##ohhey_targets_tab"))
        {
            DrawTargetUi();
            ImGui.EndTabItem();
        }
        if(ImGui.BeginTabItem("Emotes##ohhey_emotes_tab"))
        {
            DrawEmoteUi();
            ImGui.EndTabItem();
        }
    }

    private void DrawEmoteUi()
    {
        ImGui.TextUnformatted("Emote History");
        ImGui.SameLine();
        if (RightAlignedButton("Clear History"))
        {
            _emoteService.ClearEmoteHistory();
        }
        ImGui.Separator();
        const int length = EmoteService.MaxEmoteHistory + 1;
        using (ImRaii.ListBox("##ohhey_emote_list",
                   new Vector2(_textWidth,
                       length * ImGui.GetTextLineHeightWithSpacing())))
        {
            ImGui.PushItemWidth(-1);
            foreach (var emote in _emoteService.EmoteHistory)
            {
                ImGui.TextColored(KnownColor.LightGray.Vector(), $"{emote.Timestamp:HH:mm:ss} {emote.InitiatorName} used {emote.EmoteName.ToString()}");
            }
        }
    }

    private void DrawTargetUi()
    {
        ImGui.TextUnformatted("Target History");
        ImGui.SameLine();
        if (RightAlignedButton("Clear History"))
        {
            _targetService.ClearHistory();
        }
        ImGui.Separator();
        var length = Math.Clamp(_targetService.CurrentTargets.Count + _targetService.TargetHistory.Count + 1, 10, 20) +1;
        using (ImRaii.ListBox("##ohhey_target_list", new Vector2(_textWidth, length * ImGui.GetTextLineHeightWithSpacing())))
        {
            ImGui.PushItemWidth(-1);

            for (var i = _targetService.CurrentTargets.Count - 1; i >= 0; i--)
            {
                var target = _targetService.CurrentTargets[i];
                ImGui.TextUnformatted($"{target.Timestamp:HH:mm:ss} {target.Name}");
            }

            if (_targetService.CurrentTargets.Count > 0)
            {
                ImGui.Separator();
            }

            for (var i = _targetService.TargetHistory.Count - 1; i >= 0; i--)
            {
                var target = _targetService.TargetHistory[i];
                ImGui.TextColored(KnownColor.LightGray.Vector(), $"{target.Timestamp:HH:mm:ss} {target.Name}");
            }
            ImGui.PopItemWidth();
        }
    }

    private static bool RightAlignedButton(string label)
    {
        var buttonWidth = ImGui.CalcTextSize(label).X + ImGui.GetStyle().FramePadding.X * 2;
        var availableWidth = ImGui.GetContentRegionAvail().X;
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + availableWidth - buttonWidth);
        return ImGui.SmallButton(label);
    }
}
