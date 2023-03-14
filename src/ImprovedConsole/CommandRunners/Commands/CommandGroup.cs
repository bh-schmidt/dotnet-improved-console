using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandGroup : ICommand
    {
        public CommandGroup(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));

            Name = name;
            Description = description;
            OptionsName = $"{name}-options";
            Commands = new LinkedList<Command>();
            CommandGroups = new LinkedList<CommandGroup>();
            Options = new LinkedList<CommandOption>();
        }

        public CommandGroup(CommandGroup previous, string name, string description) : this(name, description)
        {
            Previous = previous;
        }

        public CommandGroup? Previous { get; }
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<Command> Commands { get; private set; }
        public IEnumerable<CommandGroup> CommandGroups { get; private set; }
        public IEnumerable<CommandOption> Options { get; private set; }
        public string OptionsName { get; set; }

        public CommandGroup AddCommand<TCommand>()
            where TCommand : Command, new()
        {
            var command = new TCommand();
            ((LinkedList<Command>)Commands).AddLast(command);
            return this;
        }

        public CommandGroup AddCommand(Command command)
        {
            ((LinkedList<Command>)Commands).AddLast(command);
            return this;
        }

        public CommandGroup AddCommand(string name, string description, Action<Command>? commandBuilder)
        {
            var command = new Command(this, name, description);
            ((LinkedList<Command>)Commands).AddLast(command);
            commandBuilder?.Invoke(command);
            return this;
        }

        protected CommandGroup AddGroup<TGroup>()
            where TGroup : CommandGroup, new()
        {
            ((LinkedList<CommandGroup>)CommandGroups).AddLast(new TGroup());
            return this;
        }

        public CommandGroup AddGroup(CommandGroup commandGroup)
        {
            ((LinkedList<CommandGroup>)CommandGroups).AddLast(commandGroup);
            return this;
        }

        public CommandGroup AddGroup(string name, string description, Action<CommandGroup>? commandGroupBuilder)
        {
            var commandGroup = new CommandGroup(this, name, description);
            ((LinkedList<CommandGroup>)CommandGroups).AddLast(commandGroup);
            commandGroupBuilder?.Invoke(commandGroup);
            return this;
        }

        public CommandGroup AddFlag(string name, string description)
        {
            var option = new CommandOption(name, description);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public CommandGroup AddOption(string name, string description, ValueLocation valueLocation = ValueLocation.SplittedBySpace)
        {
            var option = new CommandOption(name, description, valueLocation);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public CommandGroup SetOptionsName(string name)
        {
            OptionsName = name;
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

            foreach (var group in CommandGroups)
                group.Validate();

            foreach (var command in Commands)
                command.Validate();
        }

        public LinkedList<ICommand> GetCommandTree()
        {
            if (Previous is null)
            {
                var list = new LinkedList<ICommand>();
                list.AddFirst(this);
                return list;
            }

            var tree = Previous.GetCommandTree();
            tree.AddLast(this);
            return tree;
        }

        public string GetCommandTreeAsString()
        {
            var tree = GetCommandTree().Select(e => e.Name);
            return string.Join(" ", tree);
        }
    }
}
