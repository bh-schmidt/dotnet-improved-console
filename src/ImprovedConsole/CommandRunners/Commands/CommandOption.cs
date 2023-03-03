namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandOption
    {
        public CommandOption(string name, string description, bool isFlag, bool splitValueFromName)
        {
            Name = name;
            Description = description;
            IsFlag = isFlag;
            SplitValueFromName = splitValueFromName;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsFlag { get; }
        public bool SplitValueFromName { get; }

        public bool IsMatch(string[] args, int index)
        {
            if (IsFlag)
                return args[index] == Name;

            if (SplitValueFromName)
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
