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

    public class CommandMatcher
    {
        private readonly string[] arguments;
        private readonly CommandBuilder commandBuilder;
        private readonly CommandMatcherOptions options;

        public CommandMatcher(string[] arguments, CommandBuilder commandBuilder, CommandMatcherOptions options)
        {
            this.arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            this.commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            this.options = options;
        }

        public CommandMatcherResult Match()
        {
            var groupsResults = commandBuilder.CommandGroups.SelectMany(group => Match(0, group, null)).ToArray();
            var commandsResults = commandBuilder.Commands.SelectMany(command => Match(0, command, null)).ToArray();
            var results = commandsResults.Concat(groupsResults).ToArray();

            var matchedCommand = GetCommand(commandBuilder, groupsResults, commandsResults);
            var matchedGroup = GetGroup(results);

            return new CommandMatcherResult(
                commandBuilder,
                arguments,
                matchedGroup,
                matchedCommand);
        }

        private static CommandMatcherNode? GetGroup(CommandMatcherNode[] results)
        {
            var matchedGroups = results.Where(e => e.Command is CommandGroup);
            if (matchedGroups.Count() > 1)
                throw new DuplicateCommandException(matchedGroups.Select(e => e.Command));
            return matchedGroups.FirstOrDefault();
        }

        private CommandMatcherNode? GetCommand(CommandBuilder commandBuilder, CommandMatcherNode[] groupsResults, CommandMatcherNode[] commandsResults)
        {
            var matchedCommands = groupsResults.Where(e => e.Command is Command);
            if (matchedCommands.Count() > 1)
                throw new DuplicateCommandException(matchedCommands.Select(e => e.Command));

            var command = matchedCommands.FirstOrDefault();

            if (command is not null)
                return command;

            if (commandsResults.Count() > 1)
                throw new DuplicateCommandException(commandsResults.Select(e => e.Command));

            command = commandsResults.FirstOrDefault();
            
            if (command is not null)
                return command;
            if (commandBuilder.DefaultCommand is null)
                return null;

            var defaultCommands = FillMatch(0, commandBuilder.DefaultCommand, null);
            command = defaultCommands.First();

            return command;
        }

        private IEnumerable<CommandMatcherNode> Match(int currentIndex, ICommand command, CommandMatcherNode? previous)
        {
            if (arguments is null || arguments.Length <= currentIndex)
                return Enumerable.Empty<CommandMatcherNode>();

            if (command.Name != arguments[currentIndex])
                return Enumerable.Empty<CommandMatcherNode>();

            return FillMatch(currentIndex + 1, command, previous);
        }

        private IEnumerable<CommandMatcherNode> FillMatch(int currentIndex, ICommand command, CommandMatcherNode? previous)
        {
            LinkedList<ArgumentOption> options = new();
            LinkedList<ArgumentParameter> parameters = new();
            var current = new CommandMatcherNode(previous, command, options, parameters);

            var allOptions = command.Options.Distinct().ToHashSet();
            var allParameters = command is Command c ? new Queue<CommandParameter>(c.Parameters.Distinct()) : null;

            for (int index = currentIndex; index < arguments.Length; index++)
            {
                if (this.options.HandleHelp && arguments[index] is "-h" or "--help")
                {
                    CommandOption commandOption = new CommandOption(arguments[index], "Describes the command");
                    ArgumentOption argumentOption = new ArgumentOption(command, commandOption, arguments[index]);
                    options.AddLast(argumentOption);
                    return new[] { current };
                }

                var option = GetOption(command, allOptions, ref index);
                if (option is not null)
                {
                    options.AddLast(option);
                    continue;
                }

                var parameter = GetParameter(command, allParameters, index);
                if (parameter is not null)
                {
                    parameters.AddLast(parameter);
                    continue;
                }

                if (command is CommandGroup commandGroup)
                {
                    var groupResults = commandGroup.CommandGroups.SelectMany(group => Match(index, group, current));
                    if (groupResults.Any())
                        return groupResults;

                    var commandResults = commandGroup.Commands.SelectMany(command => Match(index, command, current));
                    if (commandResults.Any())
                        return commandResults;
                }
            }

            return new[] { current };
        }

        private ArgumentOption? GetOption(ICommand command, HashSet<CommandOption> options, ref int index)
        {
            var i = index;
            var option = options.FirstOrDefault(e => e.IsMatch(arguments, i));
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

            var optionValue = string.Empty;
            var startValueIndex = option.Name.Length + 1;
            if (arguments[index] != option.Name && arguments[index].Length > startValueIndex)
                optionValue = arguments[index][startValueIndex..];

            return new ArgumentOption(command, option, optionValue);
        }

        private ArgumentParameter? GetParameter(ICommand command, Queue<CommandParameter>? allParameters, int index)
        {
            if (command is not Command || allParameters is null || allParameters.Count == 0)
                return null;

            CommandParameter? parameter = allParameters.Dequeue();
            return new ArgumentParameter(command, parameter, arguments[index]);
        }
    }
}
