// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace OhHey.Services;

public sealed class WindowService : IDisposable
{
    private readonly IDalamudPluginInterface _pluginInterface;
    private readonly IPluginLog _logger;
    private readonly KeyedWindowService _keyedWindowService;
    private readonly WindowSystem _windowSystem = new("OhHey");

    public WindowService(IDalamudPluginInterface pluginInterface, IPluginLog logger,
        KeyedWindowService keyedWindowService, IEnumerable<Window> windows)
    {
        _pluginInterface = pluginInterface;
        _logger = logger;
        _keyedWindowService = keyedWindowService;

        RegisterWindows(windows);
        _pluginInterface.UiBuilder.Draw += _windowSystem.Draw;
    }

    private void RegisterWindows(IEnumerable<Window> windows)
    {
        foreach (var window in windows)
        {
            _logger.Info("Registering window: {Title}", window.WindowName);
            _windowSystem.AddWindow(window);
        }
        _keyedWindowService.RegisterKeyedWindows();
    }

    public void Dispose()
    {
        _pluginInterface.UiBuilder.Draw -= _windowSystem.Draw;
    }
}
