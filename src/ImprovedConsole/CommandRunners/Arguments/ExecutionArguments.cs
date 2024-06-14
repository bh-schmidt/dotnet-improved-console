namespace ImprovedConsole.CommandRunners.Arguments
{
    public class ExecutionArguments(
        IEnumerable<ArgumentParameter> parameters,
        IEnumerable<ArgumentOption> options,
        string[] args)
    {
        public CommandParameters Parameters { get; set; } = new CommandParameters(parameters);
        public CommandOptions Options { get; set; } = new CommandOptions(options);
        public string[] Args { get; } = args;
        public IServiceProvider? ServiceProvider { get; set; }
    }
}
