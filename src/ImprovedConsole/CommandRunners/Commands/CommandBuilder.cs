namespace ImprovedConsole.CommandRunners.Commands
{
    public class CommandBuilderOptions
    {
        public bool HandleHelp { get; set; } = true;
        public bool ValidateGroupDescription { get; set; } = true;
    }

    public class CommandBuilder : Command
    {

        public CommandBuilder() : this(new CommandBuilderOptions())
        {
        }

        public CommandBuilder(CommandBuilderOptions options)
        {
            BuilderOptions = options;
            IsDefaultCommand = true;

            WithDescription("Executes the commands");
        }

        public CommandBuilderOptions BuilderOptions { get; }

        public void Validate()
        {
            base.Validate(BuilderOptions);
        }
    }
}
