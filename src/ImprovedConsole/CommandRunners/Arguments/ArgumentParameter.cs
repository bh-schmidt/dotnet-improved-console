using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentParameter
    {
        public ArgumentParameter(ICommand command, string value)
        {
            Command = command;
            Value = value;
        }

        public ArgumentParameter(ICommand command, CommandParameter parameter, string value)
        {
            Command = command;
            Parameter = parameter;
            Value = value;
        }

        public ICommand Command { get; }
        public CommandParameter? Parameter { get; set; }
        public string Value { get; set; }
    }
}
