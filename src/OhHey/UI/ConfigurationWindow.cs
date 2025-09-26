// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI;
using JetBrains.Annotations;
using OhHey.Services;

namespace OhHey.UI;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class ConfigurationWindow : Window
{
    private readonly ConfigurationService _configService;
    private OhHeyConfiguration Config => _configService.Configuration;

    public ConfigurationWindow(ConfigurationService configService) : base("Oh Hey! Configuration##ohhey_config_window")
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

        ImGui.TextUnformatted("Notification Settings:");

        var enableTargetNotifications = Config.EnableTargetNotifications;
        if (ImGui.Checkbox("Enable target notifications", ref enableTargetNotifications))
        {
            Config.EnableTargetNotifications = enableTargetNotifications;
            _configService.Save();
        }

        TargetSoundConfig();

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

    private void TargetSoundConfig()
    {
        var soundEnabled = Config.EnableTargetSoundNotification;
        if(ImGui.Checkbox("Enable sound notification on target", ref soundEnabled))
        {
            Config.EnableTargetSoundNotification = soundEnabled;
            _configService.Save();
        }

        ImGui.TextUnformatted("Sound to play (SE.1 - SE.16)");
        var selectedIndex = _configService.Configuration.TargetSoundNotificationId;
        if (ImGui.BeginCombo("##ohhey_config_combo_target_sound", $"SE.{selectedIndex}"))
        {
            for (var i = 1; i <= 16; i++)
            {
                var isSelected = selectedIndex == i;
                if (ImGui.Selectable($"SE.{i}", isSelected))
                {
                    selectedIndex = (uint)i;
                    Config.TargetSoundNotificationId = selectedIndex;
                    _configService.Save();
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if (ImGui.ArrowButton("##ohhey_config_button_target_sound_play", ImGuiDir.Right))
        {
            UIGlobals.PlayChatSoundEffect(_configService.Configuration.TargetSoundNotificationId);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Play the selected sound effect");
        }
    }

    private void EmoteConfig()
    {
        if (!ImGui.BeginTabItem("Emotes##ohhey_config_tab_emote")) return;

        ImGui.TextUnformatted("Notification Settings:");
        ImGui.Separator();

        var enableEmoteNotifications = Config.EnableEmoteNotifications;
        if (ImGui.Checkbox("Enable emote notifications", ref enableEmoteNotifications))
        {
            Config.EnableEmoteNotifications = enableEmoteNotifications;
            _configService.Save();
        }

        EmoteSoundConfig();

        ImGui.Separator();
        ImGui.TextUnformatted("Self-emote settings:");

        var allowSelfEmote = Config.ShowSelfEmote;
        if (ImGui.Checkbox("Show self-emote in history", ref allowSelfEmote))
        {
            Config.ShowSelfEmote = allowSelfEmote;
            _configService.Save();
        }

        var notifyOnSelfEmote = Config.NotifyOnSelfEmote;
        if (ImGui.Checkbox("Notify on self-emote", ref notifyOnSelfEmote))
        {
            Config.NotifyOnSelfEmote = notifyOnSelfEmote;
            _configService.Save();
        }

        ImGui.EndTabItem();
    }

    private void EmoteSoundConfig()
    {
        var soundEnabled = Config.EnableEmoteSoundNotification;
        if(ImGui.Checkbox("Enable sound notification on emote", ref soundEnabled))
        {
            Config.EnableEmoteSoundNotification = soundEnabled;
            _configService.Save();
        }

        ImGui.TextUnformatted("Sound to play (SE.1 - SE.16)");
        var selectedIndex = _configService.Configuration.EmoteSoundNotificationId;
        if (ImGui.BeginCombo("##ohhey_config_combo_emote_sound", $"SE.{selectedIndex}"))
        {
            for (var i = 1; i <= 16; i++)
            {
                var isSelected = selectedIndex == i;
                if (ImGui.Selectable($"SE.{i}", isSelected))
                {
                    selectedIndex = (uint)i;
                    Config.EmoteSoundNotificationId = selectedIndex;
                    _configService.Save();
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        ImGui.SameLine();
        if (ImGui.ArrowButton("##ohhey_config_button_emote_sound_play", ImGuiDir.Right))
        {
            UIGlobals.PlayChatSoundEffect(_configService.Configuration.EmoteSoundNotificationId);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Play the selected sound effect");
        }
    }
}
