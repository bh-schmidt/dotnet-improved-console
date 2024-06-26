﻿using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Commands
{
    public class Command : ICommand
    {
        public Command()
        {
            OptionsName = $"options";
            Parameters = new LinkedList<CommandParameter>();
            Options = new LinkedList<CommandOption>();

        }

        public Command(CommandGroup previous) : this()
        {
            Previous = previous;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<CommandParameter> Parameters { get; private set; }
        public IEnumerable<CommandOption> Options { get; private set; }
        public Action<CommandArguments>? Handler { get; private set; }
        public CommandGroup? Previous { get; }
        public string OptionsName { get; set; }
        internal bool IsDefaultCommand { get; set; }

        public Command WithName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            Name = name;
            OptionsName = $"{name}-options";

            return this;
        }

        public Command WithDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or whitespace.", nameof(description));

            Description = description;

            return this;
        }

        public Command AddOption(string name, string description, ValueLocation valueLocation = ValueLocation.SplittedBySpace)
        {
            var option = new CommandOption(name, description, valueLocation);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public Command AddFlag(string name, string description)
        {
            var option = new CommandOption(name, description);
            ((LinkedList<CommandOption>)Options).AddLast(option);
            return this;
        }

        public Command AddParameter(string name, string description)
        {
            var parameter = new CommandParameter(name, description);
            ((LinkedList<CommandParameter>)Parameters).AddLast(parameter);
            return this;
        }

        public Command SetOptionsName(string name)
        {
            OptionsName = name;
            return this;
        }

        public Command SetHandler(Action<CommandArguments> handler)
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
    }
}
