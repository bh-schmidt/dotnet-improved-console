using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentOption
    {
        public ArgumentOption(Command command, CommandOption option, string value)
        {
            Command = command;
            Option = option;
            Value = value;
        }

        public Command Command { get; }
        public CommandOption Option { get; set; }
        public string Value { get; set; }
    }
}
