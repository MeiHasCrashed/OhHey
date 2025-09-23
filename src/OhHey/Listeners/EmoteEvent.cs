// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Text.SeStringHandling;
using Lumina.Text.ReadOnly;

namespace OhHey.Listeners;

public record EmoteEvent(
    ReadOnlySeString EmoteName,
    ushort EmoteId,
    SeString InitiatorName,
    ulong InitiatorId,
    SeString? TargetName,
    ulong TargetId,
    bool TargetSelf,
    bool InitiatorIsSelf,
    DateTime Timestamp);
