using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandGroupMatcher
    {
        private string[] arguments;

        public CommandGroupMatcher(string[] arguments)
        {
            this.arguments = arguments;
        }

        public CommandGroup? Match(IEnumerable<CommandGroup> commandGroups)
        {
            var matchedGroups = Match(0, commandGroups);

            if (matchedGroups.Count() > 1)
                throw new DuplicateCommandGroupException(matchedGroups);

            return matchedGroups.FirstOrDefault();
        }

        private IEnumerable<CommandGroup> Match(int argumentIndex, IEnumerable<CommandGroup> commandGroups)
        {
            if (arguments is null || arguments.Length <= argumentIndex)
                return Enumerable.Empty<CommandGroup>();

            var groups = commandGroups.Where(e => e.Name == arguments[argumentIndex]);
            var matched = groups.SelectMany(e => Match(argumentIndex + 1, e.CommandGroups));

            if (matched.Any())
                return matched;

            return groups;
        }
    }
}
