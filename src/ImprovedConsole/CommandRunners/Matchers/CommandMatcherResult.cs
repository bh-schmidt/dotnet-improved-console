using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherResult
    {
        public CommandMatcherResult(CommandMatcherResult? previous, ICommand command, IEnumerable<ArgumentOption> options)
        {
            Previous = previous;
            Command = command;
            Options = options;
        }

        public CommandMatcherResult(CommandMatcherResult? previous, ICommand command, IEnumerable<ArgumentOption> options, LinkedList<ArgumentParameter> parameters) : this(previous, command, options)
        {
            Parameters = parameters;
        }

        public CommandMatcherResult? Previous { get; set; }
        public ICommand Command { get; set; }
        public IEnumerable<ArgumentOption> Options { get; set; }
        public IEnumerable<ArgumentParameter> Parameters { get; set; }
    }
}
