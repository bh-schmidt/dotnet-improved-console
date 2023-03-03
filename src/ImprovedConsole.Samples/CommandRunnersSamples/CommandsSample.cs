using ImprovedConsole.CommandRunners;
using ImprovedConsole.CommandRunners.Commands;

namespace ImprovedConsole.Samples.CommandRunnersSamples
{
    public static class CommandsSample
    {
        public static void Run()
        {
            var builder = new Builder();
            var runner = new CommandRunner(builder);

            var args = new[] { "create", "--expiration", "2031-02-04", "Mike", "Anderson", "--admin", };
            //parameters are not required yet
            runner.Run(args);
        }

        class CreateUser : Command
        {
            public CreateUser() : base("create", "Creates a user")
            {
                WithParameter("name", "The user's first name");
                WithParameter("surname", "The user's second name");

                WithOption("--expiration", "Sets the expiration date.");
                WithFlag("--admin", "Creates an admin user");

                WithHandler((args) =>
                {
                    Message.Write("User created.");

                    var expiration = args.Options["--expiration"];
                    if (expiration is not null)
                        Message.Write($"User expiration set to {expiration.Value}");

                    if (args.Options["--admin"] is not null)
                        Message.Write("Setting user to be an admin.");
                });
            }
        }

        class Builder : CommandBuilder
        {
            public Builder()
            {
                AddCommand<CreateUser>();
            }
        }
    }
}
