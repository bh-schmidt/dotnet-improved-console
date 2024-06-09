using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using ImprovedConsole.CommandRunners.Handlers;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.CommandRunners
{
    public class CommandRunnerOptions
    {
        public IServiceProvider? ServiceProvider { get; set; }
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

        public Action<Command?> HelpHandler { get; set; } = (command) => { };

        public async Task RunAsync(string[] args)
        {
            var result = await Task.Factory.StartNew(() => GetMatch(args));

            if (result is null)
                return;

            ExecutionArguments arguments = CreateExecutionArguments(args, result);

            await ExecuteAsync(result.CommandNode!.Command, arguments);
        }

        public void Run(string[] args)
        {
            var result = GetMatch(args);

            if (result is null)
                return;

            var arguments = CreateExecutionArguments(args, result);
            Execute(result.CommandNode!.Command, arguments);
        }

        private CommandMatcherResult? GetMatch(string[] args)
        {
            if (args is null || args.Length == 0)
                throw new CommandNotFoundException();

            commandBuilder.Validate();

            var matcher = new CommandMatcher(args, commandBuilder);
            var result = matcher.Match();

            var command = result.CommandNode?.Command;

            if (commandBuilder.BuilderOptions.HandleHelp && result.ContainsHelpOption)
            {
                HelpHandler(command);
                return null;
            }

            if (result.CommandNode?.Command is null)
            {
                throw new CommandNotFoundException();
            }

            if (command!.Handler is null && commandBuilder.BuilderOptions.HandleHelp)
            {
                throw new WrongCommandUsageException(command);
            }

            return result;
        }

        private ExecutionArguments CreateExecutionArguments(string[] args, CommandMatcherResult result)
        {
            var argumentOptions = result.CommandNode!.GetAllOptions();
            var argumentParameters = result.CommandNode.GetAllParameters();
            var arguments = new ExecutionArguments(argumentParameters, argumentOptions, args)
            {
                ServiceProvider = options.ServiceProvider
            };
            return arguments;
        }

        private static void Execute(Command command, ExecutionArguments commandArguments)
        {
            try
            {
                if (command.Handler is SyncHandler syncHandler)
                {
                    syncHandler.Handle!(commandArguments);
                    return;
                }

                var asyncHandler = command.Handler as AsyncHandler;
                asyncHandler!.Handle(commandArguments).Wait();
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException(command, ex);
            }
        }

        private static async Task ExecuteAsync(Command command, ExecutionArguments commandArguments)
        {
            try
            {
                if (command.Handler is SyncHandler syncHandler)
                {
                    await Task.Factory.StartNew(() => syncHandler.Handle!(commandArguments));
                    return;
                }

                var asyncHandler = command.Handler as AsyncHandler;
                await asyncHandler!.Handle(commandArguments);
            }
            catch (Exception ex)
            {
                throw new CommandExecutionException(command, ex);
            }
        }
    }
}
