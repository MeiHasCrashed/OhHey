// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace OhHey;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public class OhHeyPlugin : IDalamudPlugin
{
    private readonly IServiceProvider _provider;
    public OhHeyPlugin(IDalamudPluginInterface pluginInterface)
    {
        var services = new ServiceCollection();
        services.AddSingleton(pluginInterface);

        _provider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        (_provider as IDisposable)?.Dispose();
        GC.SuppressFinalize(this);
    }
}
