using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ArgumentOption
    {
        public ArgumentOption(ICommand command, CommandOption option, string value)
        {
            Command = command;
            Option = option;
            Value = value;
        }

        public ICommand Command { get; }
        public CommandOption Option { get; set; }
        public string Value { get; set; }
    }
}
