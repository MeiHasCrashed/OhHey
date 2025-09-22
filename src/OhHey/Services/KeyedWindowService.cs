// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OhHey.UI;

namespace OhHey.Services;

public class KeyedWindowService(IPluginLog logger, IDalamudPluginInterface pluginInterface, MainWindow mainWindow)
    : IDisposable
{
    public void RegisterKeyedWindows()
    {
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;

        logger.Debug("Registered keyed windows");
    }

    private void ToggleMainWindow() => mainWindow.Toggle();

    public void Dispose()
    {
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
        GC.SuppressFinalize(this);
    }
}
