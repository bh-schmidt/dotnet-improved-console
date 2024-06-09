using ImprovedConsole.CommandRunners;
using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.Samples.CommandRunnersSamples
{
    public class CommandGroupSample
    {
        public static void Run()
        {
            var builder = new CommandBuilder();

            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithGroupDescription("Manage the users")
                    .AddFlag("--admin", "Creates an admin user")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a user")
                            .AddParameter("name", "The user's first name")
                            .AddParameter("surname", "The user's second name")
                            .AddOption("--expiration", "Sets the expiration date.")
                            .SetHandler(CreateUser.Create);
                    });
            });

            var runner = new CommandRunner(builder);

            var args = new[] { "users", "--admin", "create", "--expiration", "2031-02-04", "Mike", "Anderson", };
            runner.Run(args);
        }
        static class CreateUser
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
