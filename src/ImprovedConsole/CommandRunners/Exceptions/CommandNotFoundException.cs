using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException() { }

        public CommandNotFoundException(CommandGroup commandGroup)
        {
            CommandGroup = commandGroup;
        }

        public CommandGroup? CommandGroup { get; }
    }
}
