// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OhHey.UI;

namespace OhHey.Services;

public sealed class KeyedWindowService(IPluginLog logger, IDalamudPluginInterface pluginInterface, MainWindow mainWindow,
    ConfigurationWindow configWindow) : IDisposable
{
    public void RegisterKeyedWindows()
    {
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
        pluginInterface.UiBuilder.OpenConfigUi += ToggleConfigWindow;

        logger.Debug("Registered keyed windows");
    }

    private void ToggleMainWindow() => mainWindow.Toggle();
    private void ToggleConfigWindow() => configWindow.Toggle();

    public void OpenMainWindow() => mainWindow.IsOpen = true;

    public void Dispose()
    {
        pluginInterface.UiBuilder.OpenMainUi -= ToggleMainWindow;
        pluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigWindow;
    }
}
