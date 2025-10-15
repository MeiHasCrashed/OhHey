// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using OhHey.Listeners;

namespace OhHey.Services;

public sealed class EmoteService : IDisposable
{
    public const int MaxEmoteHistory = 10;

    private readonly IPluginLog _logger;
    private readonly EmoteListener _emoteListener;
    private readonly IChatGui _chatGui;
    private readonly ConfigurationService _configService;
    private readonly ICondition _condition;

    public LinkedList<EmoteEvent> EmoteHistory { get; } = [];

    public EmoteService(IPluginLog logger, EmoteListener emoteListener, IChatGui chatGui,
        ConfigurationService configService, ICondition condition)
    {
        _logger = logger;
        _emoteListener = emoteListener;
        _chatGui = chatGui;
        _configService = configService;
        _condition = condition;

        _emoteListener.Emote += OnEmote;
    }

    private void OnEmote(object? sender, EmoteEvent e)
    {
        _logger.Debug("Emote: {EmoteName} (ID: {EmoteId}) from {InitiatorName} (ID: {InitiatorId}) to {TargetName} (ID: {TargetId})",
            e.EmoteName.ToString(), e.EmoteId,
            e.InitiatorName.ToString(), e.InitiatorId,
            e.TargetName?.ToString() ?? "Unknown", e.TargetId);
        if (!e.TargetSelf) return;
        if (e.InitiatorIsSelf)
        {
            if (_configService.Configuration.ShowSelfEmote)
            {
                AddEmoteToHistory(e);
            }
        }
        else
        {
            AddEmoteToHistory(e);
        }
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

    public void ClearEmoteHistory() => EmoteHistory.Clear();

    private void NotifyEmoteUsed(EmoteEvent e)
    {
        if (!_configService.Configuration.EnableEmoteNotifications) return;
        if (e.InitiatorIsSelf && !_configService.Configuration.NotifyOnSelfEmote) return;
        if (!_configService.Configuration.EnableEmoteNotificationInCombat && _condition[ConditionFlag.InCombat]) return;
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

        if (!_configService.Configuration.EnableEmoteSoundNotification) return;
        UIGlobals.PlayChatSoundEffect(_configService.Configuration.EmoteSoundNotificationId);
    }

    public void Dispose()
    {
        _emoteListener.Emote -= OnEmote;
    }
}
