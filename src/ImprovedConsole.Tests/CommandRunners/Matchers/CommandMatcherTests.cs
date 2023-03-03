using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.Tests.CommandRunners.Matchers
{
    public class CommandMatcherTests
    {
        [Test]
        public void Should_match_commands_inside_groups()
        {
            var command1 = new Command("create", "Creates a new user")
                .WithOption("--expiration", "Sets user expiration")
                .WithOption("--status", "Sets user's first status", false)
                .WithParameter("name", "Sets the user name");

            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .WithFlag("--admin", "Sets user profile")
                .AddCommand(command1)
                .AddCommand(command2);

            var group2 = new CommandGroup("departments", "Manages departments");

            string[] args = new[] { "users", "--admin", "create", "John", "ignored-param", "--expiration", "2031-06-09", "--status=waiting" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1, group2, });

            result.Should().NotBeNull();
            result!.Previous.Should().NotBeNull();
            result.Command.Name.Should().Be("create");

            result.Parameters.Should().HaveCount(1);
            result.Parameters.Should().Contain(e => e.Parameter!.Name == "name" && e.Value == "John");

            result.Options.Should().HaveCount(2);
            result.Options.Should().Contain(e => e.Option.Name == "--expiration" && e.Value == "2031-06-09");
            result.Options.Should().Contain(e => e.Option.Name == "--status" && e.Value == "waiting");

            result.Previous!.Options.Should().HaveCount(1);
            result.Previous.Options.Should().Contain(e => e.Option.Name == "--admin" && e.Value == "--admin");
        }

        [Test]
        public void Should_match_commands_with_groups_inside_groups()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddGroup("admins", "Manages admins", admins =>
                {
                    admins
                        .AddCommand(command1)
                        .AddCommand(command2);
                });

            string[] args = new[] { "users", "admins", "create" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 });

            result.Should().NotBeNull();
            result!.Previous.Should().NotBeNull();
            result!.Previous!.Previous.Should().NotBeNull();

            result!.Command.Name.Should().Be("create");
        }

        [Test]
        public void Should_not_match_commands_inside_the_group()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand(command1)
                .AddCommand(command2);

            string[] args = new[] { "users", "update" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 });

            result.Should().NotBeNull();
            result!.Command.Name.Should().Be("users");
        }

        [Test]
        public void Should_not_match_group()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand(command1)
                .AddCommand(command2);

            string[] args = new[] { "departments", "create" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 });

            result.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicate_command_exception_because_group_is_duplicated()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand(command1)
                .AddCommand(command2);

            var group2 = new CommandGroup("users", "Manages users");

            string[] args = new[] { "users", "create" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());

            Assert.Catch(() =>
            {
                matcher.Match(new[] { group1, group2 });
            });
        }

        [Test]
        public void Should_not_match_because_the_command_dont_exist()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            string[] args = new[] { "update" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { command1, command2, });

            result.Should().BeNull();
        }

        [Test]
        public void Should_not_match_because_the_arguments_are_null()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var matcher = new CommandMatcher2(null!, new CommandMatcherOptions());

            var result = matcher.Match(new[] { command1, command2, });

            result.Should().BeNull();
        }

        [Test]
        public void Should_not_match_because_the_arguments_are_empty()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            string[] args = new string[] { };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { command1, command2, });

            result.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicated_command()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");
            var command3 = new Command("delete", "Delete the user");

            string[] args = new string[] { "delete" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            Assert.Catch(() =>
            {
                var result = matcher.Match(new[] { command1, command2, command3 });
            });
        }

        [Test]
        public void Should_get_the_group_with_help_option()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand(command1)
                .AddCommand(command2);

            string[] args = new[] { "users", "-h", "create" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 });

            result.Should().NotBeNull();
            result!.Command.Name.Should().Be("users");
            result.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }

        [Test]
        public void Should_get_the_command_with_help_option()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand(command1)
                .AddCommand(command2);

            string[] args = new[] { "users", "create", "-h" };
            var matcher = new CommandMatcher2(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 });

            result.Should().NotBeNull();
            result!.Command.Name.Should().Be("create");
            result.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }
    }
}
