using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.Extensions;
using System.Text;

namespace ImprovedConsole.CommandRunners.DefaultCommands
{
    public class HelpCommand(CommandBuilder commandBuilder)
    {
        public void Show(Command? matchedCommand)
        {
            var command = matchedCommand ?? commandBuilder;

            int maxArgumentSize = GetMaxArgumentSize(command);

            var builder = new StringBuilder();
            var printedBefore = PrintUsage(builder, command);
            printedBefore = PrintParameters(builder, command, maxArgumentSize, printedBefore);
            printedBefore = PrintCommands(builder, command, maxArgumentSize, printedBefore);
            printedBefore = PrintOptions(builder, command, maxArgumentSize, printedBefore);
            PrintAdditionalInformations(builder, command, printedBefore);

            ConsoleWriter.WriteLine(builder.ToString());
        }

        private static int GetMaxArgumentSize(Command? command)
        {
            int maxCommandSize = GetMaxCommandSize(command?.Commands);
            var maxParametersSize = GetMaxParametersSize(command);
            int maxCommandOptionsSize = GetMaxOptionSize(command);

            var maxArgumentSize = new[] { maxCommandSize, maxParametersSize, maxCommandOptionsSize }.Max();
            return maxArgumentSize;
        }

        private void PrintAdditionalInformations(StringBuilder builder, Command? command, bool printedBefore)
        {
            if (command is null || !command.Commands.Any())
                return;

            if (printedBefore)
                builder
                    .AppendLine()
                    .AppendLine();

            builder
                .Append(@"For more information about the commands please run");

            var tree = command.GetCommandTreeAsStringBuilder();
            builder
                .AppendLine()
                .Append(' ', 4)
                .Append(tree);

            if (tree.Length > 0)
                builder.Append(' ');

            builder.Append("[command] --help");
        }

        private static bool PrintOptions(StringBuilder builder, Command? command, int maxArgumentSize, bool printedBefore)
        {
            // why distinct???
            var commandsWithOptions = command?
                .GetCommandTree()
                .Where(ct => ct.Options.Any())
                .Distinct();

            if (commandsWithOptions.IsNullOrEmpty())
                return printedBefore;

            foreach (var optionsCommand in commandsWithOptions)
            {
                if (printedBefore)
                    builder
                        .AppendLine()
                        .AppendLine();

                builder
                    .Append(optionsCommand.OptionsName)
                    .Append(':');

                foreach (var option in optionsCommand.Options)
                {
                    builder
                        .AppendLine()
                        .Append(' ', 4)
                        .Append(option.Name.PadRight(maxArgumentSize + 4))
                        .Append(option.Description);
                }
            }

            return true;
        }

        private static bool PrintCommands(StringBuilder builder, Command? group, int maxArgumentSize, bool printedBefore)
        {
            if (group is null || group.Commands.IsNullOrEmpty())
                return printedBefore;

            if (printedBefore)
                builder
                    .AppendLine()
                    .AppendLine();

            builder
                .Append("commands:");

            var children = group.Commands;

            children = children.OrderBy(e => e.Name);

            foreach (var child in children)
            {
                builder
                    .AppendLine()
                    .Append(' ', 4)
                    .Append(child.Name.PadRight(maxArgumentSize + 4));

                if (child.Handler != null)
                {
                    builder
                        .Append(child.Description);
                }

                if (child.Commands.Any())
                {
                    if (child.Handler is not null)
                    {
                        builder
                            .AppendLine()
                            .Append(' ', 8 + maxArgumentSize);
                    }

                    builder
                        .Append(child.GroupDescription);
                }

            }

            return true;
        }

        private static bool PrintParameters(StringBuilder builder, Command? matchedCommand, int maxArgumentSize, bool printedBefore)
        {
            if (matchedCommand is null || !matchedCommand.Parameters.Any())
                return printedBefore;

            if (printedBefore)
                builder
                    .AppendLine()
                    .AppendLine();

            builder
                .Append("parameters:");

            foreach (var parameter in matchedCommand.Parameters)
                builder
                    .AppendLine()
                    .Append(' ', 4)
                    .Append(parameter.Name.PadRight(maxArgumentSize + 4))
                    .Append(parameter.Description);

            return true;
        }

        private bool PrintUsage(StringBuilder builder, Command? command)
        {
            if (command is null)
                return false;

            builder.AppendLine("usage:");

            if (command.Handler is not null)
            {
                builder
                    .Append(' ', 2)
                    .AppendLine(command.Description);

                AppendCommandTree(builder, command);

                foreach (var parameter in command.Parameters)
                {
                    builder
                        .Append(' ')
                        .Append('[')
                        .Append(parameter.Name)
                        .Append(']');
                }
            }

            if (command.Commands.Any())
            {
                if (command.Handler is not null)
                {
                    builder
                        .AppendLine()
                        .AppendLine();
                }

                builder
                    .Append(' ', 2)
                    .AppendLine(command.GroupDescription);

                AppendCommandTree(builder, command);

                builder
                    .Append(' ')
                    .Append("[command]");

                if (command.Commands.Any(c => c.Options.Any()))
                    builder.Append(" [command-options]");
            }

            return true;
        }

        private static void AppendCommandTree(StringBuilder builder, Command command)
        {
            builder.Append(' ', 3);

            foreach (var treeItem in command.GetCommandTree())
            {
                if (treeItem.Name is not null)
                {
                    builder
                        .Append(' ')
                        .Append(treeItem.Name);
                }

                if (treeItem.Options.Any())
                {
                    builder
                        .Append(' ')
                        .Append('[')
                        .Append(treeItem.OptionsName)
                        .Append(']');
                }
            }
        }

        private static int GetMaxOptionSize(Command? command)
        {
            return command?.GetCommandTree()
                .SelectMany(e => e.Options)
                .Select(e => e.Name.Length)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }

        private static int GetMaxCommandSize(IEnumerable<Command>? commands)
        {
            return commands?
                .Select(p => p.Name.Length)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }

        private static int GetMaxParametersSize(Command? command)
        {
            return command?
                .Parameters
                .Select(p => p.Name.Length)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }
    }
}
