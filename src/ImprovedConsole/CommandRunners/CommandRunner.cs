using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.CommandRunners
{
    public class CommandRunner
    {
        private readonly IEnumerable<Command> commands;
        private readonly IEnumerable<CommandGroup> commandGroups;

        public CommandRunner(CommandBuilder commandBuilder)
        {
            commandGroups = commandBuilder.CommandGroups;
            commands = commandBuilder.Commands;
        }

        public Action<IEnumerable<ICommand>, string[]> HelpHandler { get; set; } = (command, args) => { };
        public Action<Command, string[]> CommandHelpHandler { get; set; } = (command, args) => { };
        public Action<CommandGroup, string[]> CommandGroupHelpHandler { get; set; } = (group, args) => { };

        public void Run(string[] args)
        {
            if (args is null || args.Length == 0)
                throw new CommandNotFoundException();

            //var matcher = new CommandMatcher2(args, new CommandMatcherOptions());

            var command = GetCommand(args);
            var matcher = new CommandGroupMatcher(args);
            var group = matcher.Match(commandGroups);

            if (args.Contains("-h") || args.Contains("--help"))
            {
                IEnumerable<ICommand> commands;

                if (command is null)
                    commands = new[] { group! };
                else if (group is null || command.GetCommandTree().Contains(group))
                    commands = new[] { command };
                else
                    commands = new ICommand[] { command, group };

                commands = commands.Where(e => e is not null);

                HelpHandler(commands, args);
                return;
            }

            if (command is null)
            {

                if (group is null)
                    throw new CommandNotFoundException();

                if (args.Contains("-h") || args.Contains("--help"))
                {
                    CommandGroupHelpHandler(group, args);
                    return;
                }

                throw new CommandNotFoundException(group);
            }

            var argumentParameters = new LinkedList<ArgumentParameter>();
            var argumentOptions = new LinkedList<ArgumentOption>();
            Prepare(command, args, argumentOptions, argumentParameters);
            var arguments = new CommandArguments(argumentParameters, argumentOptions, args);

            if (args.Contains("-h") || args.Contains("--help"))
            {
                CommandHelpHandler(command, args);
                return;
            }

            Run(command, arguments);
        }

        private Command? GetCommand(string[] args)
        {
            var matcher = new CommandMatcher(args);

            var command = matcher.Match(commandGroups);
            if (command is not null)
                return command;

            return matcher.Match(commands);
        }

        private void Run(
            Command command,
            CommandArguments commandArguments)
        {
            if (command.Handler is null)
                throw new HandlerNotSetException(command);

            try
            {
                command.Handler(commandArguments);
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException(command, ex);
            }
        }

        private void Prepare(Command command, string[] args, LinkedList<ArgumentOption> argumentOptions, LinkedList<ArgumentParameter> argumentParameters)
        {
            if (args.Length <= 1)
                return;

            var commandTree = command.GetCommandTree().First;

            var argumentsStartIndex = command.GetCommandTree().Count();

            ArgumentOption argumentOption;
            ArgumentParameter argumentParameter;

            Queue<CommandParameter> parameters = new Queue<CommandParameter>(command.Parameters);
            var paramIndex = 0;

            for (int i = 1; i < args.Length; i++)
            {
                if (commandTree?.Next?.Value.Name == args[i])
                {
                    commandTree = commandTree.Next;
                    continue;
                }

                var option = command.Options.FirstOrDefault(e => e.IsMatch(args, i));
                if (option is not null)
                {
                    if (option.IsFlag)
                    {
                        argumentOption = new ArgumentOption(commandTree!.Value, option, args[i]);
                        argumentOptions.AddLast(argumentOption);
                        continue;
                    }

                    if (option.SplitValueFromName)
                    {
                        argumentOption = new ArgumentOption(commandTree!.Value, option, args[i + 1]);
                        argumentOptions.AddLast(argumentOption);
                        i++;
                        continue;
                    }

                    var optionValue = string.Empty;
                    if (args[i] != option.Name && args[i].Length > option.Name.Length + 1)
                        optionValue = args[i][option.Name.Length..];

                    argumentOption = new ArgumentOption(commandTree!.Value, option, optionValue);
                    argumentOptions.AddLast(argumentOption);
                    continue;
                }

                parameters.TryDequeue(out CommandParameter? parameter);
                if (parameter is not null)
                {
                    argumentParameter = new ArgumentParameter(commandTree!.Value, parameter, args[i]);
                    argumentParameters.AddLast(argumentParameter);
                    paramIndex++;
                    continue;
                }
            }
        }
    }
}
