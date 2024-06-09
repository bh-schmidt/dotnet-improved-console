using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class CommandExecutionException(Command command, Exception innerException) : Exception(GetMessage(command), innerException)
    {
        public Command Command { get; } = command;

        public static string GetMessage(Command command)
        {
            return $"An error ocurred executing the command '{command.GetCommandTreeAsString()}'";
        }
    }
}
