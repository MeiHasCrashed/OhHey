// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using OhHey.Listeners;

namespace OhHey.Services;

public sealed class TargetService : IDisposable
{
    private readonly IPluginLog _logger;
    private readonly TargetListener _targetListener;
    private readonly IChatGui _chatGui;
    private readonly IClientState _clientState;
    private readonly ConfigurationService _configService;

    public List<TargetEvent> CurrentTargets { get; } = [];

    public List<TargetEvent> TargetHistory { get; } = [];

    public TargetService(IPluginLog logger, TargetListener targetListener, IChatGui chatGui,
        ConfigurationService configService, IClientState clientState)
    {
        _logger = logger;
        _targetListener = targetListener;
        _chatGui = chatGui;
        _configService = configService;
        _clientState = clientState;

        _targetListener.Target += OnTarget;
        _targetListener.TargetRemoved += OnTargetRemoved;
    }

    private void PushToHistory(TargetEvent evt)
    {
        var position = TargetHistory.FindIndex(target => target.GameObjectId == evt.GameObjectId);
        if (position != -1)
        {
            TargetHistory.RemoveAt(position);
        }
        var targetEvent = evt with
        {
            Timestamp = DateTime.Now
        };
        if (TargetHistory.Count > 10)
        {
            TargetHistory.RemoveAt(0);
        }
        TargetHistory.Add(targetEvent);
    }

    private void OnTarget(object? sender, TargetEvent e)
    {
        _logger.Debug("Targeted by {Name} (ID: {GameObjectId} Self: {IsSelf})", e.Name, e.GameObjectId, e.IsSelf);
        if (CurrentTargets.Exists(target => target.GameObjectId == e.GameObjectId))
        {
            _logger.Warning("Received duplicate target event for {Name} ({GameObjectId})", e.Name, e.GameObjectId);
            return;
        }

        if (e.IsSelf)
        {
            if (_configService.Configuration.ShowSelfTarget)
            {
                UpdateTargetList(e);
            }
        }
        else {
            UpdateTargetList(e);
        }

        if (e.IsSelf && !_configService.Configuration.NotifyOnSelfTarget) return;
        SendNotification(e);
    }

    private void UpdateTargetList(TargetEvent e)
    {
        var position = TargetHistory.FindIndex(target => target.GameObjectId == e.GameObjectId);
        if (position != -1)
        {
            TargetHistory.RemoveAt(position);
        }
        CurrentTargets.Add(e);
    }

    private void OnTargetRemoved(object? sender, ulong e)
    {
        var position = CurrentTargets.FindIndex(target => target.GameObjectId == e);
        if (position == -1)
        {
            // This happens when we don't handle self targeting, since we still receive the target removed event.
            if (_clientState.LocalPlayer?.GameObjectId == e) return;
            _logger.Warning("Received target removed event for unknown GameObjectId {GameObjectId}", e);
            return;
        }
        var target = CurrentTargets[position];
        _logger.Debug("No longer targeted by {Name} (ID: {GameObjectId} Self: {IsSelf})",
            target.Name, target.GameObjectId, target.IsSelf);
        CurrentTargets.RemoveAt(position);
        PushToHistory(target);
    }

    public void ClearHistory() => TargetHistory.Clear();

    private void SendNotification(TargetEvent evt)
    {
        var chatMessage = new SeStringBuilder()
            .AddUiForeground("[Oh Hey!] ", 537)
            .AddUiForegroundOff()
            .Append(evt.SeName)
            .AddText(" is targeting you!")
            .Build();
        _chatGui.Print(chatMessage);
    }

    public void Dispose()
    {
        _targetListener.Target -= OnTarget;
        _targetListener.TargetRemoved -= OnTargetRemoved;
    }
}
