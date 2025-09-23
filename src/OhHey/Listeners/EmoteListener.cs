// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using Lumina.Excel.Sheets;

namespace OhHey.Listeners;

public sealed class EmoteListener : IDisposable
{
    private readonly IPluginLog _logger;
    private readonly IClientState _clientState;
    private readonly IObjectTable _objectTable;
    private readonly IDataManager _dataManager;

    public event EventHandler<EmoteEvent>? Emote;

    private delegate void OnEmoteDelegate(ulong unknown, IntPtr initiatorAddress, ushort emoteId, ulong targetId,
        ulong unknown2);

    [Signature("E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ?? 4C 89 74 24", DetourName = nameof(OnEmoteHook))]
    private readonly Hook<OnEmoteDelegate>? _onEmoteHook = null!;

    public EmoteListener(IPluginLog logger, IGameInteropProvider interopProvider, IClientState clientState, IObjectTable objectTable, IDataManager dataManager)
    {
        _logger = logger;
        _clientState = clientState;
        _objectTable = objectTable;
        _dataManager = dataManager;

        interopProvider.InitializeFromAttributes(this);
        if (_onEmoteHook is null)
        {
            _logger.Error("Failed to initialize emote hook. Any emote related features will not work.");
            _logger.Error("Please report this to Mei. Thank you <3");
            return;
        }
        _onEmoteHook.Enable();
        _logger.Debug("Emote hook initialized.");
    }

    private void OnEmoteHook(ulong unknown, IntPtr initiatorAddress, ushort emoteId, ulong targetId, ulong unknown2)
    {
        try
        {
            HandleEmoteEvent(initiatorAddress, emoteId, targetId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error executing emote hook.");
        }

        _onEmoteHook!.Original(unknown, initiatorAddress, emoteId, targetId, unknown2);
    }

    private void HandleEmoteEvent(IntPtr initiatorAddress, ushort emoteId, ulong targetId)
    {
        // We aren't logged in, somehow ??
        var localPlayer = _clientState.LocalPlayer;
        if (localPlayer is null)
        {
            return;
        }

        if (_objectTable.PlayerObjects.FirstOrDefault(chara => chara.Address == initiatorAddress) is not
            IPlayerCharacter initiator)
        {
            _logger.Warning("Failed to resolve initiator address {InitiatorAddress}. Skipping event trigger.", initiatorAddress);
            return;
        }

        var target = _objectTable.SearchById(targetId);

        var maybeEmote = _dataManager.GetExcelSheet<Emote>().GetRowOrDefault(emoteId);
        if (maybeEmote is null)
        {
            _logger.Warning("Failed to resolve emote ID {EmoteId}. Skipping event trigger.", emoteId);
            return;
        }

        var emote = maybeEmote.Value;

        var targetSelf = targetId == localPlayer.GameObjectId;
        var initiatorIsSelf = initiator.GameObjectId == localPlayer.GameObjectId;

        var emoteEvent = new EmoteEvent(emote.Name, emoteId, initiator.Name, initiator.GameObjectId, target?.Name,
            targetId, targetSelf, initiatorIsSelf, DateTime.Now);
        OnEmote(emoteEvent);
    }

    private void OnEmote(EmoteEvent emoteEvent)
    {
        var handler = Emote;
        try
        {
            handler?.Invoke(this, emoteEvent);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error invoking Emote event handlers.");
        }
    }

    public void Dispose()
    {
        _onEmoteHook?.Dispose();
    }
}
