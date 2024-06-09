using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentParameter
    {
        public ArgumentParameter(Command command, CommandParameter parameter, string value)
        {
            Command = command;
            Parameter = parameter;
            Value = value;
        }

        public Command Command { get; }
        public CommandParameter Parameter { get; set; }
        public string Value { get; set; }
    }
}
