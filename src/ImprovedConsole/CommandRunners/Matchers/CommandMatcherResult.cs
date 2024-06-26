﻿using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherResult
    {
        public CommandMatcherResult(
            CommandBuilder commandBuilder,
            string[] arguments,
            CommandMatcherNode? groupNode,
            CommandMatcherNode? commandNode)
        {
            CommandBuilder = commandBuilder;
            Arguments = arguments;
            GroupNode = groupNode;
            CommandNode = commandNode;
        }

        public CommandMatcherNode? GroupNode { get; set; }
        public CommandMatcherNode? CommandNode { get; set; }
        public string[] Arguments { get; }

        public bool ContainsHelpOption => Arguments.Contains("-h") || Arguments.Contains("--help");

        public CommandBuilder CommandBuilder { get; }
    }
}
