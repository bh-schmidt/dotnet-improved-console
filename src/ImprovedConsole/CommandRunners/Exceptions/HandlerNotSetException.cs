﻿using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class HandlerNotSetException(Command command) : Exception($"The command '{command.GetCommandTreeAsString()}' should have either a handler or sub-commands.")
    {
        public Command Command { get; } = command;
    }
}
