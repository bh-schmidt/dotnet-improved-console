using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class DuplicateCommandException : Exception
    {
        public DuplicateCommandException(IEnumerable<Command> commands) : base()
        {
            Commands = commands;
        }

        public IEnumerable<Command> Commands { get; }
    }
}
