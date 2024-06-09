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

        public async Task<bool> RunAsync(string[] args)
        {
            try
            {
                await runner.RunAsync(args);
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public bool Run(string[] args)
        {
            try
            {
                runner.Run(args);
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return false;
            }
        }

        public void HandleException(Exception exception)
        {
            switch (exception)
            {
                case GroupDescriptionNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    break;

                case DescriptionNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    break;

                case HandlerNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    break;

                case CommandExecutionException:
                    ConsoleWriter.WriteLine(exception.Message);
                    if (options.ExposeExceptionsOnConsole)
                        ConsoleWriter.WriteLine(exception.InnerException!.ToString());
                    break;

                case DuplicateCommandException:
                    ConsoleWriter.WriteLine(exception.Message);
                    break;

                case NameNotSetException:
                    ConsoleWriter.WriteLine(exception.Message);
                    break;

                case WrongCommandUsageException ex:
                    ConsoleWriter.WriteLine(exception.Message);
                    ConsoleWriter.WriteLine();
                    helpCommand.Show(ex.Command);
                    break;

                case CommandNotFoundException:
                    ConsoleWriter.WriteLine(exception.Message);
                    ConsoleWriter.WriteLine();
                    helpCommand.Show(null);
                    break;

                default:
                    break;
            };
        }
    }
}
