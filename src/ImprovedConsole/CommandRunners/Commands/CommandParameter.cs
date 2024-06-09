namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandParameter
    {
        public CommandParameter(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}
