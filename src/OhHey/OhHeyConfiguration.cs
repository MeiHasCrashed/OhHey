// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Configuration;

namespace OhHey;

[Serializable]
public class OhHeyConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool AllowSelfTarget { get; set; } = true;

    public bool AllowSelfEmote { get; set; } = false;

    public bool EnableMainWindowCloseHotkey { get; set; } = false;

}
