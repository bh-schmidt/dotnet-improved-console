namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandOption
    {
        public CommandOption(string name, string description, ValueLocation valueLocation)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));

            Name = name;
            Description = description;
            ValueLocation = valueLocation;
            IsFlag = false;
        }

        public CommandOption(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));

            if (string.IsNullOrEmpty(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));

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
