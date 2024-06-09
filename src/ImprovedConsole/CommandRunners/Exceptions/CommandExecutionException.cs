using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class CommandExecutionException : Exception
    {
        public CommandExecutionException(Command command, Exception innerException) : base("An error ocurred executing the command", innerException)
        {
            Command = command;
        }

        public Command Command { get; }
    }
}
