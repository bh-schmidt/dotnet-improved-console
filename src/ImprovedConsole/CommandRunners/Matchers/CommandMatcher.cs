using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using System.Data;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherOptions
    {
        public bool HandleHelp { get; set; }
    }

    public class CommandMatcher(string[] arguments, CommandBuilder commandBuilder)
    {
        private readonly string[] arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        private readonly CommandBuilder commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));

        public CommandMatcherResult Match()
        {
            CommandMatcherNode[] results = commandBuilder.Commands.SelectMany(command => Match(0, command, null)).ToArray();

            CommandMatcherNode? matchedCommand = GetCommand(commandBuilder, results);

            return new CommandMatcherResult(
                commandBuilder,
                arguments,
                matchedCommand);
        }

        private CommandMatcherNode? GetCommand(CommandBuilder commandBuilder, CommandMatcherNode[] commandsResults)
        {
            if (commandsResults.Length > 1)
                throw new DuplicateCommandException(commandsResults.Select(e => e.Command));

            CommandMatcherNode? command = commandsResults.FirstOrDefault();

            if (command is not null)
                return command;

            if (commandBuilder.Handler is null)
                return null;

            IEnumerable<CommandMatcherNode> defaultCommands = FillMatch(0, commandBuilder, null);
            command = defaultCommands.First();

            return command;
        }

        private IEnumerable<CommandMatcherNode> Match(int currentIndex, Command command, CommandMatcherNode? previous)
        {
            if (arguments is null || arguments.Length <= currentIndex)
                return Enumerable.Empty<CommandMatcherNode>();

            if (command.Name != arguments[currentIndex])
                return Enumerable.Empty<CommandMatcherNode>();

            return FillMatch(currentIndex + 1, command, previous);
        }

        private IEnumerable<CommandMatcherNode> FillMatch(int currentIndex, Command command, CommandMatcherNode? previous)
        {
            LinkedList<ArgumentOption> options = new();
            LinkedList<ArgumentParameter> parameters = new();
            CommandMatcherNode current = new(previous, command, options, parameters);

            HashSet<CommandOption> allOptions = command.Options.Distinct().ToHashSet();
            // why distinct???
            Queue<CommandParameter> allParameters = new(command.Parameters.Distinct());

            for (int index = currentIndex; index < arguments.Length; index++)
            {
                IEnumerable<CommandMatcherNode> commandResults = command.Commands.SelectMany(command => Match(index, command, current));
                if (commandResults.Any())
                    return commandResults;

                if (commandBuilder.BuilderOptions.HandleHelp && arguments[index] is "-h" or "--help")
                {
                    CommandOption commandOption = new(arguments[index], "Describes the command");
                    ArgumentOption argumentOption = new(command, commandOption, arguments[index]);
                    options.AddLast(argumentOption);
                    return new[] { current };
                }

                ArgumentOption? option = GetOption(command, allOptions, ref index);
                if (option is not null)
                {
                    options.AddLast(option);
                    continue;
                }

                ArgumentParameter? parameter = GetParameter(command, allParameters, index);
                if (parameter is not null)
                {
                    parameters.AddLast(parameter);
                    continue;
                }
            }

            return new[] { current };
        }

        private ArgumentOption? GetOption(Command command, HashSet<CommandOption> options, ref int index)
        {
            int i = index;
            CommandOption? option = options.FirstOrDefault(e => e.IsMatch(arguments, i));
            if (option is null)
                return null;

            options.Remove(option);

            if (option.IsFlag)
                return new ArgumentOption(command, option, arguments[i]);

            if (option.ValueLocation == ValueLocation.SplittedBySpace)
            {
                index++;
                return new ArgumentOption(command, option, arguments[index]);
            }

            string optionValue = string.Empty;
            int startValueIndex = option.Name.Length + 1;
            if (arguments[index] != option.Name && arguments[index].Length > startValueIndex)
                optionValue = arguments[index][startValueIndex..];

            return new ArgumentOption(command, option, optionValue);
        }

        private ArgumentParameter? GetParameter(Command command, Queue<CommandParameter>? allParameters, int index)
        {
            if (allParameters is null || allParameters.Count == 0)
                return null;

            CommandParameter parameter = allParameters.Dequeue();
            return new ArgumentParameter(command, parameter, arguments[index]);
        }
    }
}
