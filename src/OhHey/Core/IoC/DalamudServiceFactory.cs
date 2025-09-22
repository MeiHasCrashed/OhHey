// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.IoC;
using Dalamud.Plugin;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace OhHey.Core.IoC;

internal class DalamudServiceFactory<T>
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign)]
    [PluginService]
    private T? Service { get; set; }

    internal T Create(IServiceProvider provider)
    {
        var pluginInterface = provider.GetRequiredService<IDalamudPluginInterface>();
        pluginInterface.Inject(this);
        return Service ?? throw new InvalidOperationException($"Dalamud service of type {typeof(T).FullName} is not registered");
    }
}
