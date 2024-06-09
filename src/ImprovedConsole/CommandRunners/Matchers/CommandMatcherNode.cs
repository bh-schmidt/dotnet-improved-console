using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherNode
    {
        public CommandMatcherNode(CommandMatcherNode? previous, Command command, IEnumerable<ArgumentOption> options)
        {
            Previous = previous;
            Command = command;
            Options = options;
            Parameters = Enumerable.Empty<ArgumentParameter>();
        }

        public CommandMatcherNode(CommandMatcherNode? previous, Command command, IEnumerable<ArgumentOption> options, LinkedList<ArgumentParameter> parameters) : this(previous, command, options)
        {
            Parameters = parameters;
        }

        public CommandMatcherNode? Previous { get; set; }
        public Command Command { get; set; }
        public IEnumerable<ArgumentOption> Options { get; }
        public IEnumerable<ArgumentParameter> Parameters { get; }

        public IEnumerable<ArgumentOption> GetAllOptions()
        {
            var options = Previous is null ?
                Enumerable.Empty<ArgumentOption>() :
                Previous.GetAllOptions();

            return options.Concat(Options);
        }

        public IEnumerable<ArgumentParameter> GetAllParameters()
        {
            var parameters = Previous is null ?
                Enumerable.Empty<ArgumentParameter>() :
                Previous.GetAllParameters();

            return parameters.Concat(Parameters);
        }
    }
}
