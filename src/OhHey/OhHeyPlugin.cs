// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.ClientState.Objects;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OhHey.Core.IoC;
using OhHey.Listeners;
using OhHey.Services;
using OhHey.UI;

namespace OhHey;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public sealed class OhHeyPlugin : IDalamudPlugin
{
    private readonly IServiceProvider _provider;
    public OhHeyPlugin(IDalamudPluginInterface pluginInterface)
    {
        var services = new ServiceCollection();
        services
            .AddSingleton(pluginInterface)
            .AddDalamudService<IPluginLog>()
            .AddDalamudService<IFramework>()
            .AddDalamudService<IClientState>()
            .AddDalamudService<IObjectTable>()
            .AddDalamudService<ITargetManager>()
            .AddDalamudService<IGameInteropProvider>()
            .AddDalamudService<IDataManager>()
            .AddDalamudService<IChatGui>()
            .AddSingleton<EmoteListener>()
            .AddSingleton<EmoteService>()
            .AddSingleton<TargetListener>()
            .AddSingleton<TargetService>()
            .AddDalamudWindow<MainWindow>()
            .AddSingleton<KeyedWindowService>()
            .AddSingleton<WindowService>();

        _provider = services.BuildServiceProvider();
        _ = _provider.GetRequiredService<WindowService>();
        _ = _provider.GetRequiredService<EmoteListener>();
    }

    public void Dispose()
    {
        (_provider as IDisposable)?.Dispose();
    }
}
