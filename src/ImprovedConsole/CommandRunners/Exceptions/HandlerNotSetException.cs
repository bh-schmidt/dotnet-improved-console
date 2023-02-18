using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class HandlerNotSetException : Exception
    {
        public HandlerNotSetException(Command command) : base("The command handler for the was not set.")
        {
            Command = command;
        }

        public Command Command { get; }
    }
}
