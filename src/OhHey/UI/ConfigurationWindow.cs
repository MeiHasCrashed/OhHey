// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using JetBrains.Annotations;
using OhHey.Services;

namespace OhHey.UI;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class ConfigurationWindow : Window
{
    private readonly ConfigurationService _configService;

    private OhHeyConfiguration Config => _configService.Configuration;

    public ConfigurationWindow(ConfigurationService configService) : base("OhHey! Configuration##ohhey_config_window")
    {
        _configService = configService;
    }

    public override void Draw()
    {
        using var tabBar = ImRaii.TabBar("##ohhey_config_tab_bar");
        GeneralConfig();
        TargetConfig();
        EmoteConfig();
    }


    private void GeneralConfig()
    {
        if (!ImGui.BeginTabItem("General##ohhey_config_tab_general")) return;
        ImGui.TextUnformatted("General Settings");
        ImGui.Separator();

        var enableCloseHotkey = Config.EnableMainWindowCloseHotkey;
        if (ImGui.Checkbox("Enable closing the main window using ESC", ref enableCloseHotkey))
        {
            Config.EnableMainWindowCloseHotkey = enableCloseHotkey;
            _configService.Save();
        }

        ImGui.EndTabItem();
    }

    private void TargetConfig()
    {
        if (!ImGui.BeginTabItem("Targets##ohhey_config_tab_target")) return;

        ImGui.TextUnformatted("Target Settings");
        ImGui.Separator();

        ImGui.TextUnformatted("Self-target settings:");

        var allowSelfTarget = Config.ShowSelfTarget;
        if (ImGui.Checkbox("Show self-targeting in target list", ref allowSelfTarget))
        {
            Config.ShowSelfTarget = allowSelfTarget;
            _configService.Save();
        }

        var notifyOnSelfTarget = Config.NotifyOnSelfTarget;
        if (ImGui.Checkbox("Notify on self-target", ref notifyOnSelfTarget))
        {
            Config.NotifyOnSelfTarget = notifyOnSelfTarget;
            _configService.Save();
        }

        ImGui.EndTabItem();
    }

    private void EmoteConfig()
    {
        if (!ImGui.BeginTabItem("Emotes##ohhey_config_tab_emote")) return;

        var allowSelfEmote = Config.AllowSelfEmote;
        if (ImGui.Checkbox("Handle self emotes", ref allowSelfEmote))
        {
            Config.AllowSelfEmote = allowSelfEmote;
            _configService.Save();
        }

        ImGui.EndTabItem();
    }
}
