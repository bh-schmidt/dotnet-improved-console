using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class DuplicateCommandGroupException : Exception
    {
        public DuplicateCommandGroupException(IEnumerable<CommandGroup> commandGroups) : base()
        {
            CommandGroups = commandGroups;
        }

        public IEnumerable<CommandGroup> CommandGroups { get; }
    }
}
