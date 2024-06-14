using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Matchers;

namespace ImprovedConsole.Tests.CommandRunners.Matchers
{
    public class CommandMatcherTests
    {
        [Test]
        public void Should_match_commands_inside_groups()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder
                .AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddFlag("--admin", "Sets user profile")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .AddOption("--expiration", "Sets user expiration")
                            .AddOption("--status", "Sets user's first status", ValueLocation.SplittedByEqual)
                            .AddParameter("name", "Sets the user name");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            string[] args = ["users", "--admin", "create", "John", "ignored-param", "--expiration", "2031-06-09", "--status=waiting"];
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

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
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(admins =>
                    {
                        admins
                            .WithName("admins")
                            .WithDescription("Manages admins")
                            .AddCommand(builder =>
                            {
                                builder
                                    .WithName("create")
                                    .WithDescription("Creates a new user");
                            })
                            .AddCommand(builder =>
                            {
                                builder
                                    .WithName("delete")
                                    .WithDescription("Delete the user");
                            });
                    });
            });

            string[] args = new[] { "users", "admins", "create" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().NotBeNull();
            result!.CommandNode!.Previous.Should().NotBeNull();
            result!.CommandNode.Previous!.Previous.Should().NotBeNull();

            result!.CommandNode.Command.Name.Should().Be("create");
        }

        [Test]
        public void Should_not_match_commands_inside_the_group()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            string[] args = new[] { "users", "update" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().NotBeNull();
            result!.CommandNode!.Command.Name.Should().Be("users");
        }

        [Test]
        public void Should_not_match_group()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            string[] args = new[] { "departments", "create" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicate_command_exception_because_group_is_duplicated()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users");
            });

            commandBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manages users");
            });

            string[] args = new[] { "users", "create" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);

            Assert.Catch(() =>
            {
                matcher.Match();
            });
        }

        [Test]
        public void Should_not_match_because_the_command_dont_exist()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            string[] args = new[] { "update" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_throw_exception_because_the_arguments_are_null()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            Assert.Catch(() =>
            {
                new CommandMatcher(null!, commandBuilder);
            });
        }

        [Test]
        public void Should_not_match_because_the_arguments_are_empty()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            string[] args = new string[] { };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().BeNull();
        }

        [Test]
        public void Should_throw_duplicated_command()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            string[] args = new string[] { "delete" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            Assert.Catch(() =>
            {
                CommandMatcherResult result = matcher.Match();
            });
        }

        [Test]
        public void Should_get_the_group_with_help_option()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            commandBuilder.AddCommand(group =>
            {
                group
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            string[] args = new[] { "users", "-h", "create" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.CommandNode.Should().NotBeNull();
            result.CommandNode!.Command.Name.Should().Be("users");
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }

        [Test]
        public void Should_get_the_command_with_help_option()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });
            commandBuilder.AddCommand(delete =>
            {
                delete
                    .WithName("delete")
                    .WithDescription("Delete the user");
            });

            commandBuilder.AddCommand(group =>
            {
                group
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            string[] args = new[] { "users", "create", "-h" };
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.Should().NotBeNull();
            result.CommandNode!.Command.Name.Should().Be("create");
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "-h" && e.Value == "-h");
        }

        [Test]
        public void Should_return_default_command_because_it_did_not_found_any_group_or_command()
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.AddCommand(create =>
            {
                create
                    .WithName("create")
                    .WithDescription("Creates a new user");
            });

            commandBuilder.AddCommand(group =>
            {
                group
                    .WithName("users")
                    .WithDescription("Manages users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("delete")
                            .WithDescription("Delete the user");
                    });
            });

            commandBuilder
                .WithDescription("Handle the default command")
                .AddFlag("--list", "List all the commands")
                .SetHandler(c => { });

            string[] args = ["--list"];
            CommandMatcher matcher = new CommandMatcher(args, commandBuilder);
            CommandMatcherResult result = matcher.Match();

            result.Should().NotBeNull();
            result.CommandNode!.Command.Name.Should().BeNull();
            result.CommandNode.Options.Should().Contain(e => e.Option.Name == "--list" && e.Value == "--list");
        }
    }
}
