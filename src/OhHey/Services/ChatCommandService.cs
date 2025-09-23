// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using OhHey.UI;

namespace OhHey.Services;

public sealed class ChatCommandService : IDisposable
{
    private const string CommandName = "/ohhey";
    private readonly ICommandManager _commandManager;
    private readonly MainWindow _mainWindow;

    public ChatCommandService(ICommandManager commandManager, MainWindow mainWindow)
    {
        _commandManager = commandManager;
        _mainWindow = mainWindow;

        _commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the OhHey main window."
        });
    }

    private void OnCommand(string command, string args)
    {
        _mainWindow.Toggle();
    }


    public void Dispose()
    {
        _commandManager.RemoveHandler(CommandName);
    }
}
