// Copyright (c) 2025 MeiHasCrashed
// SPDX-License-Identifier: AGPL-3.0-or-later

using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using OhHey.UI;

namespace OhHey.Services;

public sealed class ChatCommandService : IDisposable
{
    private const string CommandName = "/ohhey";
    private readonly ICommandManager _commandManager;
    private readonly IChatGui _chatGui;
    private readonly IPluginLog _logger;
    private readonly MainWindow _mainWindow;
    private readonly ConfigurationWindow _configWindow;

    public ChatCommandService(ICommandManager commandManager, IChatGui chatGui, IPluginLog logger, MainWindow mainWindow, ConfigurationWindow configWindow)
    {
        _commandManager = commandManager;
        _chatGui = chatGui;
        _logger = logger;
        _mainWindow = mainWindow;
        _configWindow = configWindow;

        _commandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the OhHey main window if used by itself or when using '/ohhey main'.\n" +
                          "Use '/ohhey config' to open the configuration window.",
        });
    }

    private void OnCommand(string command, string argsString)
    {
        var args = argsString.Split(" ").Select(x => x.Trim()).ToArray();
        _logger.Debug("CommandInput: {Command} Args: {Args} Raw Args: {RawArgs}", command, args, argsString);
        if (args.Length == 0 || string.IsNullOrWhiteSpace(argsString))
        {
            _mainWindow.Toggle();
            return;
        }
        switch (args[0].ToLower())
        {
            case "main":
                _mainWindow.Toggle();
                break;
            case "config":
                _configWindow.Toggle();
                break;
            default:
                ShowHelp();
                break;
        }
    }

    private void ShowHelp()
    {
        var builder = new SeStringBuilder();
        builder
            .AddUiForeground("[Oh Hey!] Chat Commands: \n\n", 537).AddUiForegroundOff()
            .AddText("- ").AddUiForeground("/ohhey", 37).AddUiForegroundOff().AddText(" Opens the main window.\n")
            .AddText("- ").AddUiForeground("/ohhey main", 37).AddUiForegroundOff().AddText(" Opens the main window.\n")
            .AddText("- ").AddUiForeground("/ohhey config", 37).AddUiForegroundOff().AddText(" Opens the configuration window.\n")
            .AddText("- ").AddUiForeground("/ohhey help", 37).AddUiForegroundOff().AddText(" Shows this help message.");
        _chatGui.Print(builder.Build());
    }


    public void Dispose()
    {
        _commandManager.RemoveHandler(CommandName);
    }
}
