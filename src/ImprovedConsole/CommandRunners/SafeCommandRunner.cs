using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.DefaultCommands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners
{
    public class SafeCommandRunnerOptions : CommandRunnerOptions
    {
        public bool ExposeExceptionsOnConsole { get; set; } = false;
    }

    public class SafeCommandRunner
    {
        private readonly CommandRunner runner;
        private readonly HelpCommand helpCommand;
        private readonly SafeCommandRunnerOptions options;

        public SafeCommandRunner(CommandBuilder commandBuilder) : this(commandBuilder, new())
        {
        }

        public SafeCommandRunner(CommandBuilder commandBuilder, SafeCommandRunnerOptions options)
        {
            runner = new CommandRunner(commandBuilder, options);
            helpCommand = new HelpCommand(commandBuilder);
            runner.HelpHandler = helpCommand.Show;
            this.options = options;
        }

        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                return await runner.RunAsync(args);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public int HandleException(Exception exception)
        {
            switch (exception)
            {
                case GroupDescriptionNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    return 1;

                case DescriptionNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    return 1;

                case HandlerNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    return 1;

                case CommandExecutionException:
                    ConsoleWriter.WriteLine(exception.Message);
                    if (options.ExposeExceptionsOnConsole)
                        ConsoleWriter.WriteLine(exception.InnerException!.ToString());
                    return 1;

                case DuplicateCommandException:
                    ConsoleWriter.WriteLine(exception.Message);
                    return 1;

                case NameNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    return 1;

                case WrongCommandUsageException ex:
                    ConsoleWriter.WriteLine(exception.Message);
                    ConsoleWriter.WriteLine();
                    helpCommand.Show(ex.Command);
                    return 1;

                case CommandNotFoundException:
                    ConsoleWriter.WriteLine(exception.Message);
                    ConsoleWriter.WriteLine();
                    helpCommand.Show(null);
                    return 1;

                default:
                    return 1;
            };
        }
    }
}
