namespace ImprovedConsole.CommandRunners.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string OptionsName { get; set; }
        IEnumerable<CommandOption> Options { get; }
        CommandGroup? Previous { get; }

        LinkedList<ICommand> GetCommandTree();
        string GetCommandTreeAsString();
    }
}
