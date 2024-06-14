using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherNode(CommandMatcherNode? previous, Command command, IEnumerable<ArgumentOption> options)
    {
        public CommandMatcherNode(CommandMatcherNode? previous, Command command, IEnumerable<ArgumentOption> options, LinkedList<ArgumentParameter> parameters) : this(previous, command, options)
        {
            Parameters = parameters;
        }

        public CommandMatcherNode? Previous { get; set; } = previous;
        public Command Command { get; set; } = command;
        public IEnumerable<ArgumentOption> Options { get; } = options;
        public IEnumerable<ArgumentParameter> Parameters { get; } = Enumerable.Empty<ArgumentParameter>();

        public IEnumerable<ArgumentOption> GetAllOptions()
        {
            IEnumerable<ArgumentOption> options = Previous is null ?
                Enumerable.Empty<ArgumentOption>() :
                Previous.GetAllOptions();

            return options.Concat(Options);
        }

        public IEnumerable<ArgumentParameter> GetAllParameters()
        {
            IEnumerable<ArgumentParameter> parameters = Previous is null ?
                Enumerable.Empty<ArgumentParameter>() :
                Previous.GetAllParameters();

            return parameters.Concat(Parameters);
        }
    }
}
