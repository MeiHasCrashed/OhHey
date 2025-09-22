// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Text.SeStringHandling;

namespace OhHey.Listeners;

public record TargetEvent(ulong GameObjectId, string Name, SeString SeName, bool IsSelf);
