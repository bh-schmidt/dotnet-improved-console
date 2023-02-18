namespace ImprovedConsole.CommandRunners.Arguments
{
    public class CommandArguments
    {
        public CommandArguments(
            IEnumerable<ArgumentParameter> parameters,
            IEnumerable<ArgumentOption> options,
            string[] args)
        {
            Parameters = new CommandParameters(parameters);
            Options = new CommandOptions(options);
            Args = args;
        }

        public CommandParameters Parameters { get; set; }
        public CommandOptions Options { get; set; }
        public string[] Args { get; }
    }
}
