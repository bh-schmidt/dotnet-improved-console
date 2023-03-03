using ImprovedConsole.CommandRunners.Exceptions;

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
        public IEnumerable<Command> Commands => commands;

        protected CommandBuilder AddCommand<TCommand>()
            where TCommand : Command, new()
        {
            commands.AddLast(new TCommand());
            return this;
        }

        protected CommandBuilder AddCommand(string name, string description, Action<Command>? commandBuilder = null)
        {
            var command = new Command(this, name, description);
            commands.AddLast(command);
            commandBuilder?.Invoke(command);
            return this;
        }

        protected CommandBuilder AddGroup<TGroup>()
            where TGroup : CommandGroup, new()
        {
            commandGroups.AddLast(new TGroup());
            return this;
        }

        protected CommandBuilder AddGroup(string name, string description, Action<CommandGroup>? commandGroupBuilder = null)
        {
            var commandGroup = new CommandGroup(this, name, description);
            commandGroups.AddLast(commandGroup);
            commandGroupBuilder?.Invoke(commandGroup);
            return this;
        }

        public void Validate()
        {
            var duplicatedCommands = Commands
                .GroupBy(e => e.Name)
                .Where(e => e.Count() > 1)
                .SelectMany(e => e);

            if (duplicatedCommands.Any())
                throw new DuplicateCommandException(duplicatedCommands);

            var duplicatedGroups = CommandGroups
                .GroupBy(e => e.Name)
                .Where(e => e.Count() > 1)
                .SelectMany(e => e);

            if (duplicatedGroups.Any())
                throw new DuplicateCommandGroupException(duplicatedGroups);

            foreach (var command in commands)
                command.Validate();

            foreach (var group in commandGroups)
                group.Validate();
        }
    }
}
