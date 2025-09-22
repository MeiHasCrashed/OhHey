// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using OhHey.Services;

namespace OhHey.UI;

public class MainWindow : Window
{
    private readonly TargetService _targetService;

    public MainWindow(TargetService targetService) : base("Oh Hey!##ohhey_main_window")
    {
        _targetService = targetService;
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Target History");
        ImGui.Separator();
        var length = Math.Clamp(_targetService.CurrentTargets.Count + _targetService.TargetHistory.Count, 5, 20);
        using (ImRaii.ListBox("##ohhey_target_list", new Vector2(ImGui.GetWindowWidth() - ImGui.GetStyle().WindowPadding.X * 2, length * ImGui.GetTextLineHeightWithSpacing())))
        {
            ImGui.PushItemWidth(-1);
            foreach (var target in _targetService.CurrentTargets)
            {
                ImGui.TextUnformatted($"{target.Timestamp:HH:mm:ss} {target.Name}");
            }

            if (_targetService.CurrentTargets.Count > 0)
            {
                ImGui.Separator();
            }
            foreach (var target in _targetService.TargetHistory)
            {
                ImGui.TextColored(KnownColor.LightGray.Vector(), $"{target.Timestamp:HH:mm:ss} {target.Name}");
            }
            ImGui.PopItemWidth();
        }
    }
}
