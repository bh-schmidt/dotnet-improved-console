using ImprovedConsole.CommandRunners;
using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.Samples.CommandRunnersSamples
{
    public static class CommandsSample
    {
        public static async Task RunAsync()
        {
            var builder = new CommandBuilder();

            builder
                .AddCommand(create =>
                {
                    create
                        .WithName("create")
                        .WithDescription("Creates a user")
                        .AddParameter("name", "The user's first name")
                        .AddParameter("surname", "The user's second name")
                        .AddOption("--expiration", "Sets the expiration date.")
                        .AddFlag("--admin", "Creates an admin user")
                        .SetHandler(CreateUser.Create);
                });

            var runner = new CommandRunner(builder);

            var args = new[] { "create", "--expiration", "2031-02-04", "Mike", "Anderson", "--admin", };
            await runner.RunAsync(args);
        }

        public static class CreateUser
        {
            public static void Create(ExecutionArguments args)
            {
                Message.Write("User created.");

                var expiration = args.Options.Last("--expiration");
                if (expiration is not null)
                    Message.Write($"User expiration set to {expiration.Value}");

                if (args.Options.Last("--admin") is not null)
                    Message.Write("Setting user to be an admin.");
            }
        }
    }
}
