using ImprovedConsole.CommandRunners.Exceptions;
using System.Collections.Immutable;

namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandBuilderOptions
    {
        public string? CliName { get; set; }
    }
    
    public class CommandBuilder
    {
        private LinkedList<CommandGroup> commandGroups;
        private LinkedList<Command> commands;

        public CommandBuilder() : this(new CommandBuilderOptions())
        {
        }

        public CommandBuilder(CommandBuilderOptions options)
        {
            commandGroups = new LinkedList<CommandGroup>();
            commands = new LinkedList<Command>();
            Options = options;
        }

        public IEnumerable<CommandGroup> CommandGroups => commandGroups;
        public IEnumerable<Command> Commands => commands.ToImmutableArray();
        public Command DefaultCommand { get; private set; }
        public CommandBuilderOptions Options { get; }

        public CommandBuilder AddCommand<TCommand>()
            where TCommand : Command, new()
        {
            commands.AddLast(new TCommand());
            return this;
        }

        public CommandBuilder AddCommand(Action<Command>? commandBuilder)
        {
            var command = new Command();
            commands.AddLast(command);
            commandBuilder?.Invoke(command);
            return this;
        }

        public CommandBuilder AddDefaultCommand<TCommand>()
            where TCommand : Command, new()
        {
            DefaultCommand = new TCommand();
            DefaultCommand.IsDefaultCommand = true;
            return this;
        }

        public CommandBuilder AddDefaultCommand(Action<Command>? commandBuilder)
        {
            var command = new Command();
            command.IsDefaultCommand = true;
            DefaultCommand = command;
            commandBuilder?.Invoke(command);
            return this;
        }

        public CommandBuilder AddGroup<TGroup>()
            where TGroup : CommandGroup, new()
        {
            commandGroups.AddLast(new TGroup());
            return this;
        }

        public CommandBuilder AddGroup(Action<CommandGroup>? commandGroupBuilder)
        {
            var commandGroup = new CommandGroup();
            commandGroups.AddLast(commandGroup);
            commandGroupBuilder?.Invoke(commandGroup);
            return this;
        }

        public void Validate()
        {
            //throw new Exception("validate names and descriptions");
            //throw new Exception("default commands should not have a name");

            var duplicatedCommands = Commands
                .GroupBy(e => e.Name)
                .Where(e => e.Count() > 1)
                .FirstOrDefault();

            if (duplicatedCommands is not null && duplicatedCommands.Any())
                throw new DuplicateCommandException(duplicatedCommands);

            var duplicatedGroups = CommandGroups
                .GroupBy(e => e.Name)
                .Where(e => e.Count() > 1)
                .FirstOrDefault();

            if (duplicatedGroups is not null && duplicatedGroups.Any())
                throw new DuplicateCommandException(duplicatedGroups);

            foreach (var command in commands)
                command.Validate();

            foreach (var group in commandGroups)
                group.Validate();
        }
    }
}
