namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandParameter(string name, string description)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
    }
}
