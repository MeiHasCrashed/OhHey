// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Configuration;

namespace OhHey;

[Serializable]
public class OhHeyConfiguration : IPluginConfiguration
{
    // General Settings
    public int Version { get; set; } = 0;

    public bool EnableMainWindowCloseHotkey { get; set; } = false;

    // Target Settings
    public bool EnableTargetNotifications { get; set; } = true;

    public bool EnableTargetSoundNotification { get; set; } = false;

    public uint TargetSoundNotificationId { get; set; } = 1;

    public bool ShowSelfTarget { get; set; } = true;

    public bool NotifyOnSelfTarget { get; set; } = false;

    public bool EnableTargetNotificationInCombat { get; set; } = false;

    // Emote Settings
    public bool EnableEmoteNotifications { get; set; } = true;

    public bool EnableEmoteSoundNotification { get; set; } = false;

    public uint EmoteSoundNotificationId { get; set; } = 1;

    public bool ShowSelfEmote { get; set; } = false;

    public bool NotifyOnSelfEmote { get; set; } = false;

    public bool EnableEmoteNotificationInCombat { get; set; } = false;

}
