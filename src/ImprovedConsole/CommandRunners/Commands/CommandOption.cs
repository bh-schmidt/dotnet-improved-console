namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandOption
    {
        public CommandOption(string name, string description, ValueLocation valueLocation)
        {
            Name = name;
            Description = description;
            ValueLocation = valueLocation;
            IsFlag = false;
        }

        public CommandOption(string name, string description)
        {
            Name = name;
            Description = description;
            IsFlag = true;
        }

        public string Name { get; }
        public string Description { get; }
        public ValueLocation ValueLocation { get; }
        public bool IsFlag { get; }

        public bool IsMatch(string[] args, int index)
        {
            if (IsFlag)
                return args[index] == Name;

            if (ValueLocation == ValueLocation.SplittedBySpace)
            {
                var splitIndex = index + 1;
                return args[index] == Name && args.Length > splitIndex;
            }

            return 
                args[index] == Name ||
                args[index].StartsWith($"{Name}=");
        }
    }
}
