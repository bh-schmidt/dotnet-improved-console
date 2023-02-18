using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.DefaultCommands;
using ImprovedConsole.CommandRunners.Exceptions;
using System.Text;

namespace ImprovedConsole.CommandRunners
{
    public class SafeCommandRunner
    {
        private readonly CommandRunner runner;

        public SafeCommandRunner(CommandBuilder CommandBuilder)
        {
            runner = new CommandRunner(CommandBuilder);

            runner.HelpHandler = (group, command) =>
            {
                HelpCommand.Show(group, command);
            };
        }

        public void Run(string[] args)
        {
            try
            {
                runner.Run(args);
            }
            catch (DuplicateCommandException ex)
            {
                Handle(ex);
            }
            catch (HandlerNotSetException ex)
            {
                Handle(ex);
            }
            catch (CommandExecutionException ex)
            {
                Handle(ex);
            }
            catch (CommandNotFoundException ex)
            {
                Handle(ex);
            }
        }

        private void Handle(CommandNotFoundException ex)
        {
            if (ex.CommandGroup is not null)
            {
                var tree = ex.CommandGroup.GetCommandTreeAsString();
                ConsoleWriter.WriteLine($"Command not found. Try using {tree} -h/--help to list the commands.");
                return;
            }

            ConsoleWriter.WriteLine(@"Command not found. Try using -h or --help to list the commands.");
        }

        private static void Handle(DuplicateCommandException ex)
        {
            var builder = new StringBuilder()
                .Append("The following commands are facing conflict");

            var lastIndex = ex.Commands.Count() - 1;
            var index = 0;

            foreach (var command in ex.Commands)
            {
                builder
                    .AppendLine()
                    .Append(' ', 4)
                    .Append(command.GetCommandTreeAsString())
                    .AppendLine()
                    .Append(' ', 8)
                    .Append("of type ")
                    .Append(command.GetType().FullName);

                if (index < lastIndex)
                    builder.AppendLine();

                index++;
            }

            ConsoleWriter.WriteLine(builder.ToString());
        }

        private static void Handle(HandlerNotSetException ex)
        {
            ConsoleWriter.WriteLine($"The handler for the command '{ex.Command.GetCommandTreeAsString()}' was not set");
        }

        private static void Handle(CommandExecutionException ex)
        {
            ConsoleWriter.WriteLine($"An error ocurred executing the command '{ex.Command.GetCommandTreeAsString()}'");
            ConsoleWriter.WriteLine(ex.InnerException!.ToString());
        }
    }
}
