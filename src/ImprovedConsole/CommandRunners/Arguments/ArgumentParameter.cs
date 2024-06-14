using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentParameter(Command command, CommandParameter parameter, string value)
    {
        public Command Command { get; } = command;
        public CommandParameter Parameter { get; set; } = parameter;
        public string Value { get; set; } = value;
    }
}
