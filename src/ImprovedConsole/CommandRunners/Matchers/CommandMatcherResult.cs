namespace ImprovedConsole.CommandRunners.Matchers
{
    public class CommandMatcherResult
    {
        public CommandMatcherResult(CommandMatcherNode? group, CommandMatcherNode? command)
        {
            CommandNode = command;
            GroupNode = group;
        }

        public CommandMatcherNode? GroupNode { get; set; }
        public CommandMatcherNode? CommandNode { get; set; }

        public bool ContainsHelpOption =>
            (GroupNode is not null && GroupNode.Options.Any(e => e.Option.Name is "-h" or "--help")) ||
            (CommandNode is not null && CommandNode.Options.Any(e => e.Option.Name is "-h" or "--help"));
    }
}
