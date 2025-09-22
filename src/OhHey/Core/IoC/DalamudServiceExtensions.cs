// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Interface.Windowing;
using Microsoft.Extensions.DependencyInjection;

namespace OhHey.Core.IoC;

public static class DalamudServiceExtensions
{
    public static IServiceCollection AddDalamudService<T>(this IServiceCollection services) where T : class
    {
        services.AddSingleton<T>(provider => new DalamudServiceFactory<T>().Create(provider));
        return services;
    }

    public static IServiceCollection AddDalamudWindow<T>(this IServiceCollection services) where T : Window
    {
        services.AddSingleton<T>();
        services.AddSingleton<Window>(provider => provider.GetRequiredService<T>());
        return services;
    }
}
