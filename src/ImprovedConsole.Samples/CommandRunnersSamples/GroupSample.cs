using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners;

namespace ImprovedConsole.Samples.CommandRunnersSamples
{
    public class GroupSample
    {
        public static void Run()
        {
            var builder = new Builder();
            var runner = new CommandRunner(builder);

            var args = new[] { "users", "--admin", "create", "--expiration", "2031-02-04", "Mike", "Anderson", };
            //parameters are not required yet
            runner.Run(args);
        }

        class UsersGroup : CommandGroup
        {
            public UsersGroup() : base("users", "Manage the users")
            {
                AddCommand<CreateUser>();
                AddFlag("--admin", "Creates an admin user");
            }
        }

        class CreateUser : Command
        {
            public CreateUser() : base("create", "Creates a user")
            {
                AddParameter("name", "The user's first name");
                AddParameter("surname", "The user's second name");

                AddOption("--expiration", "Sets the expiration date.");

                SetHandler((args) =>
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
                AddGroup<UsersGroup>();
            }
        }
    }
}
