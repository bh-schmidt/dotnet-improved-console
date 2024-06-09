using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class DescriptionNotSetException(Command command) : Exception($"The description for the command '{command.GetCommandTreeAsString()}' was not set")
    {
        public Command Command { get; } = command;
    }
}
