using ImprovedConsole.CommandRunners.Commands;
using System.ComponentModel.Design;
using System.Text;

namespace ImprovedConsole.CommandRunners.DefaultCommands
{
    public class HelpCommand
    {
        public static void Show(CommandGroup? group, Command? command)
        {
            var maxParametersSize = GetMaxParametersSize(command);
            int maxCommandSize = GetMaxCommandSize(group);
            int maxOptionsSize = GetMaxOptionSize(command);

            var maxArgumentSize = new[] { maxParametersSize, maxOptionsSize, maxCommandSize }.Max();

            var builder = new StringBuilder()
                .Append("usage:");

            PrintUsage(builder, command);

            if (command is not null && group is not null)
                builder.AppendLine();

            PrintUsage(builder, group);

            if (command is not null && command.Parameters.Any())
            {
                builder
                    .AppendLine()
                    .AppendLine()
                    .Append("parameters:");

                foreach (var parameter in command.Parameters)
                    builder
                        .AppendLine()
                        .Append(' ', 4)
                        .Append(parameter.Name.PadRight(maxArgumentSize + 4))
                        .Append(parameter.Description);
            }

            if (group is not null && (group.Commands.Any() || group.CommandGroups.Any()))
            {
                builder
                    .AppendLine()
                    .AppendLine()
                    .Append("commands:");

                var children = group.Commands.AsEnumerable<ICommand>()
                    .Concat(group.CommandGroups)
                    .OrderBy(e => e.Name);

                foreach (var child in children)
                    builder
                        .AppendLine()
                        .Append(' ', 4)
                        .Append(child.Name.PadRight(maxArgumentSize + 4))
                        .Append(child.Description);
            }

            var optionsCommands = command?
                .GetCommandTree()
                .Where(ct => ct.Options.Any())
                .Distinct();

            if (optionsCommands is not null && optionsCommands.Any())
            {
                foreach (var optionsCommand in optionsCommands)
                {
                    builder
                        .AppendLine()
                        .AppendLine()
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
            }

            if (group is not null && (group.Commands.Any() || group.CommandGroups.Any()))
            {
                builder
                    .AppendLine()
                    .AppendLine()
                    .Append(@"For more information about the commands please run");

                builder
                    .AppendLine()
                    .Append(' ', 4);

                foreach (var tree in group.GetCommandTree())
                {
                    builder
                        .Append(tree.Name)
                        .Append(' ');
                }

                builder.Append("[command] --help");
            }

            ConsoleWriter.WriteLine(builder.ToString());
        }

        private static void PrintUsage(StringBuilder builder, ICommand? matchedCommand)
        {
            if (matchedCommand is null)
                return;

            builder
                .AppendLine()
                .Append(' ', 2)
                .AppendLine(matchedCommand.Description)
                .Append(' ', 3);

            foreach (var treeItem in matchedCommand.GetCommandTree())
            {
                builder
                    .Append(' ')
                    .Append(treeItem.Name);

                if (treeItem.Options.Any())
                    builder
                        .Append(' ')
                        .Append('[')
                        .Append(treeItem.OptionsName)
                        .Append(']');
            }

            if (matchedCommand is Command c)
                foreach (var parameter in c.Parameters)
                    builder
                        .Append(' ')
                        .Append('[')
                        .Append(parameter.Name)
                        .Append(']');

            if (matchedCommand is CommandGroup cg)
            {
                builder
                    .Append(' ')
                    .Append("[command]");

                if (cg.Commands.Any(c => c.Options.Any()) || cg.CommandGroups.Any(cg => cg.Options.Any()))
                    builder.Append(" [command-options]");
            }
        }

        private static int GetMaxOptionSize(Command? command)
        {
            return command?.Options
                .Select(e => e.Name.Length)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }

        private static int GetMaxCommandSize(CommandGroup? group)
        {
            return group?.Commands
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
