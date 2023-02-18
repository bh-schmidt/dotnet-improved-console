using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.Tests.CommandRunners.Matchers
{
    public class CommandMatcherTests
    {
        [Test]
        public void Should_match_commands_inside_groups()
        {
            var group1 = new CommandGroup("users", "Manages users")
                .AddFlag("--admin", "Sets user profile")
                .AddCommand("create", "Creates a new user", create =>
                {
                    create
                        .AddOption("--expiration", "Sets user expiration")
                        .AddOption("--status", "Sets user's first status", ValueLocation.SplittedByEqual)
                        .AddParameter("name", "Sets the user name");
                })
                .AddCommand("delete", "Delete the user", builder => { });

            var group2 = new CommandGroup("departments", "Manages departments");

            string[] args = new[] { "users", "--admin", "create", "John", "ignored-param", "--expiration", "2031-06-09", "--status=waiting" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1, group2, }, Enumerable.Empty<Command>(), null);

            result.GroupNode.Should().BeNull();
            result.CommandNode.Should().NotBeNull();

            result.CommandNode!.Previous.Should().NotBeNull();
            result.CommandNode.Command.Name.Should().Be("create");

            result.CommandNode.Parameters.Should().HaveCount(1);
            result.CommandNode.Parameters.Should().Contain(e => e.Parameter!.Name == "name" && e.Value == "John");

            result.CommandNode.Options.Should().HaveCount(2);
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "--expiration" && e.Value == "2031-06-09");
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "--status" && e.Value == "waiting");

            result.CommandNode.Previous!.Options.Should().HaveCount(1);
            result.CommandNode.Previous.Options.Should().Contain(e => e.Option.Name == "--admin" && e.Value == "--admin");
        }

        [Test]
        public void Should_match_commands_with_groups_inside_groups()
        {
            var group1 = new CommandGroup("users", "Manages users")
                .AddGroup("admins", "Manages admins", admins =>
                {
                    admins
                        .AddCommand("create", "Creates a new user", builder => { })
                        .AddCommand("delete", "Delete the user", builder => { });
                });

            string[] args = new[] { "users", "admins", "create" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, Enumerable.Empty<Command>(), null);

            result.CommandNode.Should().NotBeNull();
            result!.CommandNode!.Previous.Should().NotBeNull();
            result!.CommandNode.Previous!.Previous.Should().NotBeNull();

            result!.CommandNode.Command.Name.Should().Be("create");
        }

        [Test]
        public void Should_not_match_commands_inside_the_group()
        {
            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand("create", "Creates a new user", builder => { })
                .AddCommand("delete", "Delete the user", builder => { });

            string[] args = new[] { "users", "update" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, Enumerable.Empty<Command>(), null);

            result.GroupNode.Should().NotBeNull();
            result.CommandNode.Should().BeNull();

            result!.GroupNode!.Command.Name.Should().Be("users");
        }

        [Test]
        public void Should_not_match_group()
        {
            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand("create", "Creates a new user", builder => { })
                .AddCommand("delete", "Delete the user", builder => { });

            string[] args = new[] { "departments", "create" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, Enumerable.Empty<Command>(), null);

            result.GroupNode.Should().BeNull();
            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicate_command_exception_because_group_is_duplicated()
        {
            var group1 = new CommandGroup("users", "Manages users");
            var group2 = new CommandGroup("users", "Manages users");

            string[] args = new[] { "users", "create" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());

            Assert.Catch(() =>
            {
                matcher.Match(new[] { group1, group2 }, Enumerable.Empty<Command>(), null);
            });
        }

        [Test]
        public void Should_not_match_because_the_command_dont_exist()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            string[] args = new[] { "update" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(Enumerable.Empty<CommandGroup>(), new[] { command1, command2, }, null);

            result.GroupNode.Should().BeNull();
            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_not_match_because_the_arguments_are_null()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var matcher = new CommandMatcher(null!, new CommandMatcherOptions());

            var result = matcher.Match(Enumerable.Empty<CommandGroup>(), new[] { command1, command2, }, null);

            result.GroupNode.Should().BeNull();
            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_not_match_because_the_arguments_are_empty()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            string[] args = new string[] { };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(Enumerable.Empty<CommandGroup>(), new[] { command1, command2, }, null);

            result.GroupNode.Should().BeNull();
            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicated_command()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");
            var command3 = new Command("delete", "Delete the user");

            string[] args = new string[] { "delete" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            Assert.Catch(() =>
            {
                var result = matcher.Match(Enumerable.Empty<CommandGroup>(), new[] { command1, command2, command3 }, null);
            });
        }

        [Test]
        public void Should_get_the_group_with_help_option()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand("create", "Creates a new user", builder => { })
                .AddCommand("delete", "Delete the user", builder => { });

            string[] args = new[] { "users", "-h", "create" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, Enumerable.Empty<Command>(), null);

            result.GroupNode.Should().NotBeNull();
            result.CommandNode.Should().BeNull();

            result.GroupNode!.Command.Name.Should().Be("users");
            result.GroupNode.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }

        [Test]
        public void Should_get_the_command_with_help_option()
        {
            var command1 = new Command("create", "Creates a new user");
            var command2 = new Command("delete", "Delete the user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand("create", "Creates a new user", builder => { })
                .AddCommand("delete", "Delete the user", builder => { });

            string[] args = new[] { "users", "create", "-h" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, Enumerable.Empty<Command>(), null);

            result.Should().NotBeNull();
            result.CommandNode!.Command.Name.Should().Be("create");
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }

        [Test]
        public void Should_return_default_command_because_it_did_not_found_any_group_or_command()
        {
            var command1 = new Command("create", "Creates a new user");

            var group1 = new CommandGroup("users", "Manages users")
                .AddCommand("create", "Creates a new user", builder => { })
                .AddCommand("delete", "Delete the user", builder => { });

            var defaultCommand = new Command("Handle the default command")
                .AddFlag("--list", "List all the commands");

            string[] args = new[] { "--list" };
            var matcher = new CommandMatcher(args, new CommandMatcherOptions());
            var result = matcher.Match(new[] { group1 }, new[] { command1 }, defaultCommand);

            result.Should().NotBeNull();
            result.CommandNode!.Command.Name.Should().Be("default");
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "--list" && e.Value == "--list");
        }
    }
}
