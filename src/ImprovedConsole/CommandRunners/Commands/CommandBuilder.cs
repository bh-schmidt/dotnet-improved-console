using ImprovedConsole.CommandRunners.Exceptions;
using System.Collections.Immutable;

namespace ImprovedConsole.CommandRunners.Commands
{
    public abstract class CommandBuilder
    {
        private LinkedList<CommandGroup> commandGroups;
        private LinkedList<Command> commands;

        public CommandBuilder()
        {
            commandGroups = new LinkedList<CommandGroup>();
            commands = new LinkedList<Command>();
        }

        public IEnumerable<CommandGroup> CommandGroups => commandGroups;
        public IEnumerable<Command> Commands => commands.ToImmutableArray();
        public Command DefaultCommand { get; private set; }

        protected CommandBuilder AddCommand<TCommand>()
            where TCommand : Command, new()
        {
            commands.AddLast(new TCommand());
            return this;
        }

        protected CommandBuilder AddCommand(Command command)
        {
            commands.AddLast(command);
            return this;
        }


        protected CommandBuilder AddCommand(string name, string description, Action<Command>? commandBuilder)
        {
            var command = new Command(name, description);
            commands.AddLast(command);
            commandBuilder?.Invoke(command);
            return this;
        }

        protected CommandBuilder AddDefaultCommand<TCommand>()
            where TCommand : Command, new()
        {
            DefaultCommand = new TCommand();
            return this;
        }

        protected CommandBuilder AddDefaultCommand(Command command)
        {
            DefaultCommand = command;
            return this;
        }


        protected CommandBuilder AddDefaultCommand(string description, Action<Command>? commandBuilder)
        {
            var command = new Command(description);
            DefaultCommand = command;
            commandBuilder?.Invoke(command);
            return this;
        }

        protected CommandBuilder AddGroup<TGroup>()
            where TGroup : CommandGroup, new()
        {
            commandGroups.AddLast(new TGroup());
            return this;
        }

        public CommandBuilder AddGroup(CommandGroup commandGroup)
        {
            commandGroups.AddLast(commandGroup);
            return this;
        }

        protected CommandBuilder AddGroup(string name, string description, Action<CommandGroup>? commandGroupBuilder)
        {
            var commandGroup = new CommandGroup(name, description);
            commandGroups.AddLast(commandGroup);
            commandGroupBuilder?.Invoke(commandGroup);
            return this;
        }

        public void Validate()
        {
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
