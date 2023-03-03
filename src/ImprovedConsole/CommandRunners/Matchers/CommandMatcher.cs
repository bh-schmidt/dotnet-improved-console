using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcher
    {
        private readonly string[] arguments;

        public CommandMatcher(string[] arguments)
        {
            this.arguments = arguments;
        }

        public Command? Match(IEnumerable<Command> commands)
        {
            var matched = Match(0, commands);

            if (matched.Count() > 1)
                throw new DuplicateCommandException(commands);

            return matched.FirstOrDefault();
        }

        public Command? Match(IEnumerable<CommandGroup> commandGroups)
        {
            var commands = Match(0, commandGroups);

            if (commands.Count() > 1)
                throw new DuplicateCommandException(commands);

            return commands.FirstOrDefault();
        }

        private IEnumerable<Command> Match(int argumentIndex, IEnumerable<Command> commands)
        {
            if (arguments is null || arguments.Length <= argumentIndex)
                return Enumerable.Empty<Command>();

            return commands.Where(e => e.Name == arguments[argumentIndex]);
        }

        private IEnumerable<Command> Match(int argumentIndex, IEnumerable<CommandGroup> commandGroups)
        {
            if (arguments is null || arguments.Length <= argumentIndex)
                return Enumerable.Empty<Command>();

            var matchedGroups = commandGroups.Where(e => e.Name == arguments[argumentIndex]);

            var commands = matchedGroups.SelectMany(e => Match(argumentIndex + 1, e.CommandGroups));
            if (commands.Any())
                return commands;

            return matchedGroups.SelectMany(e => Match(argumentIndex + 1, e.Commands));
        }
    }
}
