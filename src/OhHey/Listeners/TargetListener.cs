// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Plugin.Services;

namespace OhHey.Listeners;

public sealed class TargetListener : IDisposable
{
    private const int UpdateMilliseconds = 100;

    private readonly IPluginLog _logger;
    private readonly IFramework _framework;
    private readonly IClientState _clientState;
    private readonly IObjectTable _objectTable;
    private readonly ITargetManager _targetManager;

    private readonly Stopwatch _updateStopwatch = new();
    private readonly List<ulong> _lastTargetingPlayers = [];

    public event EventHandler<TargetEvent>? Target;
    public event EventHandler<ulong>? TargetRemoved;

    public TargetListener(IPluginLog logger, IFramework framework, IClientState clientState, IObjectTable objectTable, ITargetManager targetManager)
    {
        _logger = logger;
        _framework = framework;
        _clientState = clientState;
        _objectTable = objectTable;
        _targetManager = targetManager;
        _framework.Update += FrameworkUpdate;
        _updateStopwatch.Start();
    }

    private void FrameworkUpdate(IFramework framework)
    {
        if (_clientState.IsPvP) return;
        if (_updateStopwatch.ElapsedMilliseconds < UpdateMilliseconds) return;
        CheckForTargets();
        _updateStopwatch.Restart();
    }

    private void CheckForTargets()
    {
        var currentPlayer = _clientState.LocalPlayer;
        if (currentPlayer == null) return;

        var targetingPlayers = _objectTable.PlayerObjects
            .Where(chara => chara is IPlayerCharacter &&
                            chara.GameObjectId == currentPlayer.GameObjectId
                ? IsLocalPlayerTargeting(currentPlayer.GameObjectId)
                : chara.TargetObjectId == currentPlayer.GameObjectId
            )
            .ToList();

        // If we have no players currently targeting us, we can clear the old list directly.
        if (targetingPlayers.Count == 0)
        {
            _lastTargetingPlayers.ForEach(OnTargetRemoved);
            _lastTargetingPlayers.Clear();
            return;
        }

        foreach (var targetingPlayer in targetingPlayers.Where(targetingPlayer => !_lastTargetingPlayers.Contains(targetingPlayer.GameObjectId)))
        {
            _lastTargetingPlayers.Add(targetingPlayer.GameObjectId);
            var targetEvent = new TargetEvent(targetingPlayer.GameObjectId, targetingPlayer.Name.ToString(),
                targetingPlayer.Name, targetingPlayer.GameObjectId == currentPlayer.GameObjectId, DateTime.Now);
            OnTarget(targetEvent);
        }

        var targetingPlayerIds = targetingPlayers.Select(player => player.GameObjectId).ToArray();
        foreach (var lastTargetingPlayer in _lastTargetingPlayers.ToArray())
        {
            if(targetingPlayerIds.Contains(lastTargetingPlayer)) continue;
            _lastTargetingPlayers.Remove(lastTargetingPlayer);
            OnTargetRemoved(lastTargetingPlayer);
        }
    }

    private bool IsLocalPlayerTargeting(ulong targetId)
    {
        return (_targetManager.Target ?? _targetManager.SoftTarget)?.GameObjectId == targetId;
    }

    private void OnTarget(TargetEvent @event)
    {
        var handler = Target;
        try
        {
            handler?.Invoke(this, @event);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in Target event handler");
        }
    }

    private void OnTargetRemoved(ulong targetId)
    {
        var handler = TargetRemoved;
        try
        {
            handler?.Invoke(this, targetId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in TargetRemoved event handler");
        }
    }


    public void Dispose()
    {
        _framework.Update -= FrameworkUpdate;
    }
}
