using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherResult(
        CommandBuilder commandBuilder,
        string[] arguments,
        CommandMatcherNode? commandNode)
    {
        public CommandMatcherNode? CommandNode { get; set; } = commandNode;
        public string[] Arguments { get; } = arguments;

        public bool ContainsHelpOption => Arguments.Contains("-h") || Arguments.Contains("--help");

        public CommandBuilder CommandBuilder { get; } = commandBuilder;
    }
}
