// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Plugin.Services;
using OhHey.Listeners;

namespace OhHey.Services;

public sealed class EmoteService : IDisposable
{
    public const int MaxEmoteHistory = 10;

    private readonly IPluginLog _logger;
    private readonly EmoteListener _emoteListener;

    public LinkedList<EmoteEvent> EmoteHistory { get; } = [];

    public EmoteService(IPluginLog logger, EmoteListener emoteListener)
    {
        _logger = logger;
        _emoteListener = emoteListener;

        _emoteListener.Emote += OnEmote;
    }

    private void OnEmote(object? sender, EmoteEvent e)
    {
        _logger.Debug("Emote: {EmoteName} (ID: {EmoteId}) from {InitiatorName} (ID: {InitiatorId}) to {TargetName} (ID: {TargetId})",
            e.EmoteName.ToString(), e.EmoteId,
            e.InitiatorName.ToString(), e.InitiatorId,
            e.TargetName?.ToString() ?? "Unknown", e.TargetId);
        if (!e.TargetSelf) return;
        AddEmoteToHistory(e);
    }

    private void AddEmoteToHistory(EmoteEvent e)
    {
        if (EmoteHistory.Count >= MaxEmoteHistory)
        {
            EmoteHistory.RemoveLast();
        }
        EmoteHistory.AddFirst(e);
    }

    public void Dispose()
    {
        _emoteListener.Emote -= OnEmote;
    }
}
