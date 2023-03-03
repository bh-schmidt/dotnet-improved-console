using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Commands
{
    public class Command : ICommand
    {
        public Command(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));

            Name = name;
            Description = description;
            OptionsName = $"{name}-options";
            Parameters = new LinkedList<CommandParameter>();
            Options = new LinkedList<CommandOption>();
        }

        public Command(CommandBuilder? commandRegistrator, string name, string description) : this(name, description)
        {
            CommandRegistrator = commandRegistrator;
        }


        public Command(CommandBuilder? commandRegistrator, CommandGroup previous, string name, string description) : this(commandRegistrator, name, description)
        {
            Previous = previous;
        }

        public CommandBuilder? CommandRegistrator { get; }
        public string Name { get; }
        public string Description { get; }
        public IEnumerable<CommandParameter> Parameters { get; private set; }
        public IEnumerable<CommandOption> Options { get; private set; }
        public Action<CommandArguments>? Handler { get; private set; }
        public CommandGroup? Previous { get; }
        public string OptionsName { get; set; }

        public Command WithOption(string name, string description, bool splitValueFromName = true)
        {
            var option = new CommandOption(name, description, false, splitValueFromName);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public Command WithFlag(string name, string description)
        {
            var option = new CommandOption(name, description, true, true);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public Command WithParameter(string name, string description)
        {
            var parameter = new CommandParameter(name, description);
            ((LinkedList<CommandParameter>)Parameters).AddLast(parameter);
            return this;
        }

        public Command WithOptionsName(string name)
        {
            OptionsName = name;
            return this;
        }

        public Command WithHandler(Action<CommandArguments> handler)
        {
            Handler = handler;
            return this;
        }

        public void Validate()
        {
            if (Handler is null)
                throw new HandlerNotSetException(this);
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

        public string ParametersString()
        {
            if (!Parameters.Any())
                return string.Empty;

            var parameters = Parameters.Select(e => e.Name);
            return $"<{string.Join("> <", parameters)}>";
        }

        public string OptionsString()
        {
            if (!Options.Any())
                return string.Empty;

            var options = Options.Select(e => e.Name);
            return $"    - {string.Join("\n    - ", options)}>";
        }
    }
}
