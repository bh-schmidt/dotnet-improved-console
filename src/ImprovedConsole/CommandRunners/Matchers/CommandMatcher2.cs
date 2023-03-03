using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using System.Data;

namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherOptions
    {
        public bool HandleHelp { get; set; } = true;
    }

    public class CommandMatcher2
    {
        private readonly string[] arguments;
        private readonly CommandMatcherOptions options;

        public CommandMatcher2(string[] arguments, CommandMatcherOptions options)
        {
            this.arguments = arguments;
            this.options = options;
        }

        public CommandMatcherResult? Match(IEnumerable<Command> commands)
        {
            var results = commands.SelectMany(command => Match(0, command, null));

            if (results.Count() > 1)
                throw new DuplicateCommandException(results.Select(e => e.Command));

            return results.FirstOrDefault();
        }

        public CommandMatcherResult? Match(IEnumerable<CommandGroup> commandGroups)
        {
            var results = commandGroups.SelectMany(group => Match(0, group, null));

            if (results.Count() > 1)
                throw new DuplicateCommandException(results.Select(e => e.Command));

            return results.FirstOrDefault();
        }

        private IEnumerable<CommandMatcherResult> Match(int argumentIndex, Command command, CommandMatcherResult? previous)
        {
            if (arguments is null || arguments.Length <= argumentIndex || command.Name != arguments[argumentIndex])
                return Enumerable.Empty<CommandMatcherResult>();

            LinkedList<ArgumentOption> options = new();
            LinkedList<ArgumentParameter> parameters = new();
            var current = new CommandMatcherResult(previous, command, options, parameters);

            var allOptions = command.Options.Distinct().ToHashSet();
            var allParameters = new Queue<CommandParameter>(command.Parameters.Distinct());

            for (int index = argumentIndex + 1; index < arguments.Length; index++)
            {
                if (this.options.HandleHelp && arguments[index] is "-h" or "--help")
                {
                    CommandOption commandOption = new CommandOption(arguments[index], "Describes the command", true, false);
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
            }

            return new[] { current };
        }

        private IEnumerable<CommandMatcherResult> Match(int argumentIndex, CommandGroup commandGroup, CommandMatcherResult? previous)
        {
            if (arguments is null || arguments.Length <= argumentIndex || commandGroup.Name != arguments[argumentIndex])
                return Enumerable.Empty<CommandMatcherResult>();

            LinkedList<ArgumentOption> options = new();
            var current = new CommandMatcherResult(previous, commandGroup, options);

            var allOptions = commandGroup.Options.Distinct().ToHashSet();

            for (int index = argumentIndex + 1; index < arguments.Length; index++)
            {
                if (this.options.HandleHelp && arguments[index] is "-h" or "--help")
                {
                    CommandOption commandOption = new CommandOption(arguments[index], "Describes the command", true, false);
                    ArgumentOption argumentOption = new ArgumentOption(commandGroup, commandOption, arguments[index]);
                    options.AddLast(argumentOption);
                    return new[] { current };
                }

                var option = GetOption(commandGroup, allOptions, ref index);
                if (option is not null)
                {
                    options.AddLast(option);
                    continue;
                }

                var groupResults = commandGroup.CommandGroups.SelectMany(group => Match(index, group, current));
                if (groupResults.Any())
                    return groupResults;

                var commandResults = commandGroup.Commands.SelectMany(command => Match(index, command, current));
                if (commandResults.Any())
                    return commandResults;
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

            if (option.SplitValueFromName)
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

        private ArgumentParameter? GetParameter(Command command, Queue<CommandParameter> allParameters, int index)
        {
            if (allParameters.Count == 0)
                return null;

            CommandParameter? parameter = allParameters.Dequeue();
            return new ArgumentParameter(command, parameter, arguments[index]);
        }
    }
}
