using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.CommandRunners
{
    public class CommandRunnerOptions
    {
        public bool HandleHelp { get; set; } = true;
    }

    public class CommandRunner
    {
        private readonly CommandBuilder commandBuilder;
        private readonly CommandRunnerOptions options;

        public CommandRunner(CommandBuilder commandBuilder)
        {
            this.commandBuilder = commandBuilder;
            options = new CommandRunnerOptions();
        }

        public CommandRunner(CommandBuilder commandBuilder, CommandRunnerOptions options)
        {
            this.commandBuilder = commandBuilder;
            this.options = options;
        }

        public Action<CommandGroup?, Command?> HelpHandler { get; set; } = (group, command) => { };

        public void Run(string[] args)
        {
            if (args is null || args.Length == 0)
                throw new CommandNotFoundException();

            commandBuilder.Validate();

            var matcher = new CommandMatcher(
                args,
                commandBuilder,
                new CommandMatcherOptions
                {
                    HandleHelp = options.HandleHelp
                });
            var result = matcher.Match();

            if (options.HandleHelp && result.ContainsHelpOption)
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
