// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin.Services;
using OhHey.Listeners;

namespace OhHey.Services;

public class TargetService
{
    private readonly IPluginLog _logger;
    private readonly TargetListener _targetListener;

    public List<TargetEvent> CurrentTargets { get; } = [];

    public TargetService(IPluginLog logger, TargetListener targetListener)
    {
        _logger = logger;
        _targetListener = targetListener;

        _targetListener.Target += OnTarget;
        _targetListener.TargetRemoved += OnTargetRemoved;
    }

    private void OnTarget(object? sender, TargetEvent e)
    {
        if (CurrentTargets.Exists(target => target.GameObjectId == e.GameObjectId))
        {
            _logger.Warning("Received duplicate target event for {Name} ({GameObjectId})", e.Name, e.GameObjectId);
            return;
        }
        CurrentTargets.Add(e);
        _logger.Debug("Targeted by {Name} (ID: {GameObjectId} Self: {IsSelf})", e.Name, e.GameObjectId, e.IsSelf);
    }

    private void OnTargetRemoved(object? sender, ulong e)
    {
        var position = CurrentTargets.FindIndex(target => target.GameObjectId == e);
        if (position == -1)
        {
            _logger.Warning("Received target removed event for unknown GameObjectId {GameObjectId}", e);
            return;
        }
        _logger.Debug("No longer targeted by {Name} (ID: {GameObjectId} Self: {IsSelf})", CurrentTargets[position].Name, CurrentTargets[position].GameObjectId, CurrentTargets[position].IsSelf);
        CurrentTargets.RemoveAt(position);
    }
}
