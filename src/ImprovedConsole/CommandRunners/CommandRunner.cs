using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.CommandRunners
{
    public class CommandRunner
    {
        private readonly CommandBuilder commandBuilder;

        public CommandRunner(CommandBuilder commandBuilder)
        {
            this.commandBuilder = commandBuilder;
        }

        public Action<CommandGroup?, Command?> HelpHandler { get; set; } = (group, command) => { };

        public void Run(string[] args)
        {
            if (args is null || args.Length == 0)
                throw new CommandNotFoundException();

            commandBuilder.Validate();

            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(commandBuilder.CommandGroups, commandBuilder.Commands, null);

            if (result.ContainsHelpOption)
            {
                HelpHandler((CommandGroup?)result.GroupNode?.Command, (Command?)result.CommandNode?.Command);
                return;
            }

            if (result.CommandNode is null)
            {
                if (result.GroupNode is null)
                    throw new CommandNotFoundException();

                throw new CommandNotFoundException((CommandGroup)result.GroupNode.Command);
            }

            var argumentOptions = result.CommandNode.GetAllOptions();
            var argumentParameters = result.CommandNode.GetAllParameters();
            var arguments = new CommandArguments(argumentParameters, argumentOptions, args);

            Run((Command)result.CommandNode.Command, arguments);
        }

        private void Run(Command command, CommandArguments commandArguments)
        {
            try
            {
                command.Handler!(commandArguments);
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException(command, ex);
            }
        }
    }
}
