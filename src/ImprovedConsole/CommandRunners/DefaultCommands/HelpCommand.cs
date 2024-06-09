using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.Extensions;
using System.Text;

namespace ImprovedConsole.CommandRunners.DefaultCommands
{
    public class HelpCommand
    {
        private readonly CommandBuilder commandBuilder;

        public HelpCommand(CommandBuilder commandBuilder)
        {
            this.commandBuilder = commandBuilder;
        }

        public void Show(CommandGroup? matchedGroup, Command? matchedCommand)
        {
            var hashAnyMatch = (matchedGroup is not null || matchedCommand is not null);

            var command = matchedCommand;
            var group = matchedGroup;

            if (
                (matchedCommand?.IsDefaultCommand == true || !hashAnyMatch) &&
                (commandBuilder.CommandGroups.Any() || commandBuilder.Commands.Any()))
            {
                group = new CommandGroup()
                    .WithDescription("Executes the commands");

                foreach (var item in commandBuilder.CommandGroups)
                    group.AddGroup(item);

                foreach (var item in commandBuilder.Commands)
                    group.AddCommand(item);
            }

            IEnumerable<ICommand> allCommands = GetAllCommands(command, group);

            int maxArgumentSize = GetMaxArgumentSize(group, command);

            var builder = new StringBuilder();
            var printedBefore = PrintUsage(builder, allCommands);
            printedBefore = PrintParameters(builder, command, maxArgumentSize, printedBefore);
            printedBefore = PrintCommands(builder, group, maxArgumentSize, printedBefore);
            printedBefore = PrintOptions(builder, group, command, maxArgumentSize, printedBefore);
            PrintMatchedGroupAdditionalInformations(builder, group, printedBefore);

            ConsoleWriter.WriteLine(builder.ToString());
        }

        private static IEnumerable<ICommand> GetAllCommands(Command? command, CommandGroup? group)
        {
            var allCommands = Enumerable.Empty<ICommand>();
            if (command is not null)
                allCommands = allCommands.Append(command);

            if (group is not null)
                allCommands = allCommands.Append(group);
            return allCommands;
        }

        private static int GetMaxArgumentSize(CommandGroup? group, Command? command)
        {
            int maxCommandSize = GetMaxCommandSize(group?.Commands);
            int maxGroupSize = GetMaxCommandSize(group?.CommandGroups);
            var maxParametersSize = GetMaxParametersSize(command);
            int maxCommandOptionsSize = GetMaxOptionSize(command);
            int maxGroupOptionsSize = GetMaxOptionSize(group);

            var maxArgumentSize = new[] { maxCommandSize, maxGroupSize, maxParametersSize, maxCommandOptionsSize, maxGroupOptionsSize }.Max();
            return maxArgumentSize;
        }

        private void PrintMatchedGroupAdditionalInformations(StringBuilder builder, CommandGroup? group, bool printedBefore)
        {
            if (group is null || !group.Commands.Any() && !group.CommandGroups.Any())
                return;

            if (printedBefore)
                builder
                    .AppendLine()
                    .AppendLine();

            builder
                .Append(@"For more information about the commands please run");

            builder
                .AppendLine()
                .Append(' ', 4);

            foreach (var treeItem in group.GetCommandTree())
            {
                builder
                    .Append(treeItem.Name)
                    .Append(' ');
            }

            if (!commandBuilder.Options.CliName.IsNullOrEmpty())
            {
                builder
                    .Append(commandBuilder.Options.CliName)
                    .Append(' ');
            }

            builder.Append("[command] --help");
        }

        private static bool PrintOptions(StringBuilder builder, CommandGroup? matchedGroup, Command? matchedCommand, int maxArgumentSize, bool printedBefore)
        {
            var commandsWithOptions = Enumerable.Empty<ICommand>();

            var commandTree = matchedCommand?.GetCommandTree();
            var groupTree = matchedGroup?.GetCommandTree();

            if (!commandTree.IsNullOrEmpty())
                commandsWithOptions = commandTree;

            if (!groupTree.IsNullOrEmpty())
                commandsWithOptions = commandsWithOptions.Concat(groupTree);

            commandsWithOptions = commandsWithOptions?
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

        private static bool PrintCommands(StringBuilder builder, CommandGroup? group, int maxArgumentSize, bool printedBefore)
        {
            if (group is null || (group.Commands.IsNullOrEmpty() && group.CommandGroups.IsNullOrEmpty()))
                return printedBefore;

            if (printedBefore)
                builder
                    .AppendLine()
                    .AppendLine();

            builder
                .Append("commands:");

            var children = Enumerable.Empty<ICommand>();

            if (group.Commands is not null)
                children = children.Concat(group.Commands);

            if (group.CommandGroups is not null)
                children = children.Concat(group.CommandGroups);

            children = children.OrderBy(e => e.Name);

            foreach (var child in children)
                builder
                    .AppendLine()
                    .Append(' ', 4)
                    .Append(child.Name.PadRight(maxArgumentSize + 4))
                    .Append(child.Description);

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

        private bool PrintUsage(StringBuilder builder, IEnumerable<ICommand> allCommands)
        {
            if (allCommands.IsNullOrEmpty())
                return false;

            builder.Append("usage:");

            var appendLine = false;

            foreach (var command in allCommands)
            {
                if (appendLine)
                    builder.AppendLine();

                PrintUsage(builder, command);

                appendLine = true;
            }

            return true;
        }

        private void PrintUsage(StringBuilder builder, ICommand? matchedCommand)
        {
            if (matchedCommand is null)
                return;

            builder
                .AppendLine()
                .Append(' ', 2)
                .AppendLine(matchedCommand.Description)
                .Append(' ', 3);

            if (!commandBuilder.Options.CliName.IsNullOrEmpty())
            {
                builder
                    .Append(' ')
                    .Append(commandBuilder.Options.CliName);
            }

            foreach (var treeItem in matchedCommand.GetCommandTree())
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

            if (matchedCommand is Command c)
            {
                foreach (var parameter in c.Parameters)
                {
                    builder
                        .Append(' ')
                        .Append('[')
                        .Append(parameter.Name)
                        .Append(']');
                }
            }

            if (matchedCommand is CommandGroup cg)
            {
                builder
                    .Append(' ')
                    .Append("[command]");

                if (cg.Commands.Any(c => c.Options.Any()) || cg.CommandGroups.Any(cg => cg.Options.Any()))
                    builder.Append(" [command-options]");
            }
        }

        private static int GetMaxOptionSize(ICommand? command)
        {
            return command?.GetCommandTree()
                .SelectMany(e => e.Options)
                .Select(e => e.Name.Length)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }

        private static int GetMaxCommandSize(IEnumerable<ICommand>? commands)
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
