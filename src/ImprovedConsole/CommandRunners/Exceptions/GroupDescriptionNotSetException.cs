using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class GroupDescriptionNotSetException(Command command) : Exception($"The group description for the command '{command.GetCommandTreeAsString()}' was not set")
    {
        public Command Command { get; } = command;
    }
}
