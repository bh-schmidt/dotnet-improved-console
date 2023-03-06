namespace ImprovedConsole.CommandRunners.Commands
{
    public class InlineCommandBuilder : CommandBuilder
    {
        public new CommandBuilder AddGroup(string name, string description, Action<CommandGroup>? commandGroupBuilder = null)
        {
            return base.AddGroup(name, description, commandGroupBuilder);
        }

        public new CommandBuilder AddCommand(string name, string description, Action<Command>? commandBuilder = null)
        {
            return base.AddCommand(name, description, commandBuilder);
        }
    }
}
