using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentOption(Command command, CommandOption option, string value)
    {
        public Command Command { get; } = command;
        public CommandOption Option { get; set; } = option;
        public string Value { get; set; } = value;
    }
}
