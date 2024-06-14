using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Exceptions;
using ImprovedConsole.CommandRunners.Handlers;
using ImprovedConsole.Extensions;
using System.Diagnostics;
using System.Text;

namespace ImprovedConsole.CommandRunners.Commands
{
    [DebuggerDisplay("{GetCommandTreeAsString()}")]
    public class Command
    {
        public Command()
        {
            Name = null!;
            Description = null!; ;
            OptionsName = $"options";
            Parameters = new LinkedList<CommandParameter>();
            Options = new LinkedList<CommandOption>();
            Commands = new LinkedList<Command>();
        }

        public Command(Command previous) : this()
        {
            Previous = previous;
        }

        public Command? Previous { get; internal set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string GroupDescription { get; private set; } = "Executes the sub-commands";

        public IEnumerable<CommandParameter> Parameters { get; private set; }
        public IEnumerable<CommandOption> Options { get; private set; }

        public IEnumerable<Command> Commands { get; private set; }
        public ICommandHandler? Handler { get; private set; }

        public string OptionsName { get; set; }
        internal bool IsDefaultCommand { get; set; }

        public virtual Command WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            Name = name;
            OptionsName = $"{name}-options";

            return this;
        }

        public virtual Command WithDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));

            Description = description;

            return this;
        }

        public virtual Command WithGroupDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));

            GroupDescription = description;

            return this;
        }

        public Command AddCommand<TCommand>()
            where TCommand : Command, new()
        {
            TCommand command = new TCommand
            {
                Previous = this
            };
            ((LinkedList<Command>)Commands).AddLast(command);
            return this;
        }

        public Command AddCommand(Action<Command>? commandBuilder)
        {
            Command command = new Command(this);
            ((LinkedList<Command>)Commands).AddLast(command);
            commandBuilder?.Invoke(command);
            return this;
        }

        public virtual Command AddParameter(string name, string description)
        {
            CommandParameter parameter = new CommandParameter(name, description);
            ((LinkedList<CommandParameter>)Parameters).AddLast(parameter);
            return this;
        }

        public virtual Command AddOption(string name, string description, ValueLocation valueLocation = ValueLocation.SplittedBySpace)
        {
            CommandOption option = new CommandOption(name, description, valueLocation);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public virtual Command AddFlag(string name, string description)
        {
            CommandOption option = new CommandOption(name, description);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public virtual Command SetOptionsName(string name)
        {
            OptionsName = name;
            return this;
        }

        public virtual Command SetHandler(Func<ExecutionArguments, int> handler)
        {
            Handler = new CommandHandler(args =>
            {
                return Task.FromResult(handler(args));
            });
            return this;
        }


        public virtual Command SetHandler(Action<ExecutionArguments> handler)
        {
            Handler = new CommandHandler(args =>
            {
                handler(args);
                return Task.FromResult(0);
            });
            return this;
        }

        public virtual Command SetHandler(Func<ExecutionArguments, Task<int>> handler)
        {
            Handler = new CommandHandler(async args =>
            {
                return await handler(args);
            });
            return this;
        }

        public virtual Command SetHandler(Func<ExecutionArguments, Task> handler)
        {
            Handler = new CommandHandler(async args =>
            {
                await handler(args);
                return 0;
            });
            return this;
        }

        public virtual Command SetHandler<THandler>() where THandler : ICommandHandler
        {
            Handler = new InjectedCommandHandler(typeof(THandler));
            return this;
        }

        public virtual void Validate(CommandBuilderOptions builderOptions)
        {
            if (!IsDefaultCommand && Name.IsNullOrEmpty())
                throw new NameNotSetException(this);

            if (builderOptions.HandleHelp && Handler is not null && Description.IsNullOrEmpty())
                throw new DescriptionNotSetException(this);

            if (builderOptions.ValidateGroupDescription && Commands.Any() && GroupDescription.IsNullOrEmpty())
                throw new GroupDescriptionNotSetException(this);

            if (Handler is null && !Commands.Any())
                throw new HandlerNotSetException(this);

            IGrouping<string, Command>? duplicatedCommands = Commands
                .GroupBy(e => e.Name)
                .Where(e => e.Count() > 1)
                .FirstOrDefault();

            if (duplicatedCommands is not null && duplicatedCommands.Any())
                throw new DuplicateCommandException(duplicatedCommands);

            foreach (Command command in Commands)
                command.Validate(builderOptions);
        }

        public virtual LinkedList<Command> GetCommandTree()
        {
            if (Previous is null)
            {
                LinkedList<Command> list = new LinkedList<Command>();
                list.AddFirst(this);
                return list;
            }

            LinkedList<Command> tree = Previous.GetCommandTree();
            tree.AddLast(this);
            return tree;
        }

        public virtual StringBuilder GetCommandTreeAsStringBuilder()
        {
            IEnumerable<string> tree = GetCommandTree()
                .Select(e => e.Name)
                .Where(e => !e.IsNullOrEmpty());

            return new StringBuilder()
                .AppendJoin(" ", tree);
        }

        public virtual string GetCommandTreeAsString()
        {
            IEnumerable<string> tree = GetCommandTree()
                .Select(e => e.Name)
                .Where(e => !e.IsNullOrEmpty());

            return string.Join(" ", tree);
        }
    }
}
