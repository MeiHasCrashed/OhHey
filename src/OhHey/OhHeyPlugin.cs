// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace OhHey;

public class OhHeyPlugin : IDalamudPlugin
{
    public OhHeyPlugin(IPluginLog logger)
    {
        logger.Info("OhHey loading started.");
    }

    public void Dispose()
    {

    }
}
