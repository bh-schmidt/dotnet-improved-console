using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class WrongCommandUsageException(Command command) : Exception("Wrong command usage.")
    {
        public Command Command { get; } = command;
    }
}
