// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics;
using System.Text;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Plugin.Services;

namespace OhHey.Services;

public sealed class DtrBarService : IDisposable
{
    private readonly IPluginLog _logger;
    private readonly ConfigurationService _configService;
    private readonly TargetService _targetService;
    private readonly KeyedWindowService _keyedWindowService;
    private readonly IDtrBar _dtrBar;
    private readonly IFramework _framework;


    private const string DtrBarTitle = "Oh Hey!";

    private readonly Stopwatch _updateStopwatch = new();

    private IDtrBarEntry? _dtrBarEntry;

    public DtrBarService(IPluginLog logger, ConfigurationService configService, TargetService targetService, KeyedWindowService keyedWindowService, IDtrBar dtrBar, IFramework framework)
    {
        _logger = logger;
        _configService = configService;
        _targetService = targetService;
        _keyedWindowService = keyedWindowService;
        _dtrBar = dtrBar;
        _framework = framework;

        _configService.ConfigurationChanged += OnConfigurationChanged;
        _framework.Update += OnFrameworkUpdate;

        if (configService.Configuration.ShowInDtrBar)
        {
            _logger.Info("DtrBar entry enabled in configuration.");
            EnableDtrBarEntry();
        }
    }

    private void EnableDtrBarEntry()
    {
        if (_dtrBarEntry is not null)
        {
            _logger.Warning("DtrBar entry for OhHey already exists, skipping initialization.");
            return;
        }

        _dtrBarEntry = _dtrBar.Get(DtrBarTitle, $"\uE05E 0");
        _dtrBarEntry.OnClick += OnDtrClick;
        _logger.Debug("DtrBar entry created.");
        _updateStopwatch.Restart();
    }

    private void DisableDtrBarEntry()
    {
        _updateStopwatch.Reset();
        if (_dtrBarEntry is null)
        {
            _logger.Debug("DtrBar entry for OhHey does not exist, skipping removal.");
            return;
        }
        _dtrBarEntry.OnClick -= OnDtrClick;
        _dtrBar.Remove(DtrBarTitle);
        _dtrBarEntry = null;
        _logger.Debug("DtrBar entry removed.");
    }

    private void OnDtrClick(DtrInteractionEvent obj)
    {
        _keyedWindowService.OpenMainWindow();
    }


    private void OnFrameworkUpdate(IFramework framework)
    {
        if (_dtrBarEntry is null) return;
        if (_updateStopwatch.ElapsedMilliseconds < 1000) return;
        _dtrBarEntry.Text = $"\uE05E {_targetService.CurrentTargets.Count}";
        var baseString = _targetService.CurrentTargets.Count == 0 ? "Oh Hey - Not being targeted" : "Oh Hey! - Currently targeted by:\n";
        var sb = new StringBuilder(baseString);
        var count = 0;
        foreach (var target in _targetService.CurrentTargets)
        {
            if (count >= 9)
            {
                sb.AppendLine($"- and {_targetService.CurrentTargets.Count - count} more...");
                break;
            }
            sb.AppendLine($"- {target.Name}");
            count++;
        }
        _dtrBarEntry.Tooltip = sb.ToString();
        _updateStopwatch.Restart();
    }


    private void OnConfigurationChanged(object? sender, OhHeyConfiguration e)
    {
        if (e.ShowInDtrBar && _dtrBarEntry is null)
        {
            EnableDtrBarEntry();
        }
        else if (!e.ShowInDtrBar && _dtrBarEntry is not null)
        {
            DisableDtrBarEntry();
        }
    }

    public void Dispose()
    {
        _configService.ConfigurationChanged -= OnConfigurationChanged;
        _framework.Update -= OnFrameworkUpdate;
        DisableDtrBarEntry();
    }
}
