using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class DuplicateCommandException : Exception
    {
        public DuplicateCommandException(IEnumerable<ICommand> commands) : base()
        {
            Commands = commands;
        }

        public IEnumerable<ICommand> Commands { get; }
    }
}
