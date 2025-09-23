// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using OhHey.Listeners;

namespace OhHey.Services;

public sealed class EmoteService : IDisposable
{
    public const int MaxEmoteHistory = 10;

    private readonly IPluginLog _logger;
    private readonly EmoteListener _emoteListener;
    private readonly IChatGui _chatGui;

    public LinkedList<EmoteEvent> EmoteHistory { get; } = [];

    public EmoteService(IPluginLog logger, EmoteListener emoteListener, IChatGui chatGui)
    {
        _logger = logger;
        _emoteListener = emoteListener;
        _chatGui = chatGui;

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
        NotifyEmoteUsed(e);
    }

    private void AddEmoteToHistory(EmoteEvent e)
    {
        if (EmoteHistory.Count >= MaxEmoteHistory)
        {
            EmoteHistory.RemoveLast();
        }
        EmoteHistory.AddFirst(e);
    }

    private void NotifyEmoteUsed(EmoteEvent e)
    {
        var chatMessage = new SeStringBuilder()
            .AddUiForeground("[Oh Hey!] ", 537)
            .AddUiForegroundOff()
            .Append(e.InitiatorName)
            .AddText($" used ")
            .AddUiForeground(e.EmoteName.ToString(), 1)
            .AddUiForegroundOff()
            .AddText(" on you!")
            .Build();
        _chatGui.Print(chatMessage);
    }

    public void Dispose()
    {
        _emoteListener.Emote -= OnEmote;
    }
}
