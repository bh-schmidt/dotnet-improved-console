using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.Tests.CommandRunners.Commands
{
    public class CommandBuilderTests
    {
        [Test]
        public void Should_register_groups_and_commands()
        {
            CommandBuilder builder = new CommandBuilder();

            builder.AddCommand(command =>
            {
                command
                    .WithName("cache")
                    .WithDescription("Enable or disable the cache")
                    .SetHandler(args => { });
            });

            builder.AddCommand(department =>
            {
                department
                    .WithName("department")
                    .WithDescription("manage the departments")
                    .AddCommand(users =>
                    {
                        users
                            .WithName("users")
                            .WithDescription("manage the users")
                            .SetHandler(args => { });
                    });

                department.AddCommand(command =>
                {
                    command
                        .WithName("create")
                        .WithDescription("creates a new department")
                        .SetHandler(args => { });
                });
            });

            builder.Validate();
            builder.Commands.Should().HaveCount(2);
        }

        [Test]
        public void Should_throw_duplicate_command_because_there_is_two_commands_with_the_same_name()
        {
            CommandBuilder builder = new CommandBuilder();
            builder.AddCommand(cache =>
            {
                cache
                    .WithName("cache")
                    .WithDescription("Enable or disable the cache");
            });
            builder.AddCommand(cache =>
            {
                cache
                    .WithName("cache")
                    .WithDescription("Enable or disable the cache");
            });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                builder.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_group_because_there_is_two_command_groups_with_the_same_name()
        {
            CommandBuilder builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users");
            });
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users");
            });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                builder.Validate();
            });
        }

        [Test]
        public void Should_throw_handler_not_set()
        {
            CommandBuilder builder = new CommandBuilder();
            builder.AddCommand(cache =>
            {
                cache
                    .WithName("cache")
                    .WithDescription("Enable or disable the cache");
            });

            Assert.Catch<HandlerNotSetException>(() =>
            {
                builder.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_because_there_is_two_commands_with_the_same_name_inside_the_same_group()
        {
            CommandBuilder builder = new CommandBuilder();
            builder.AddCommand(department =>
            {
                department
                    .WithName("department")
                    .WithDescription("manage the departments")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("creates a new department");
                    })
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("creates a new department");
                    });
            });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                builder.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_group_because_there_is_two_command_groups_with_the_same_name_inside_the_same_group()
        {
            CommandBuilder builder = new CommandBuilder();
            builder
                .AddCommand(department =>
                {
                    department
                        .WithName("department")
                        .WithDescription("manage the departments")
                        .AddCommand(users =>
                        {
                            users
                                .WithName("users")
                                .WithDescription("manage the users");
                        })
                        .AddCommand(users =>
                        {
                            users
                                .WithName("users")
                                .WithDescription("manage the users");
                        });
                });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                builder.Validate();
            });
        }
    }
}
