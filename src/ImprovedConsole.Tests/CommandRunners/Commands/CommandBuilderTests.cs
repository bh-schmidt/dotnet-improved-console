using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;
using System.Text.RegularExpressions;

namespace ImprovedConsole.Tests.CommandRunners.Commands
{
    public class CommandBuilderTests
    {
        [Test]
        public void Should_register_groups_and_commands()
        {
            var builder = new CommandBuilder();

            builder.AddCommand(command =>
            {
                command
                    .WithName("cache")
                    .WithDescription("Enable or disable the cache")
                    .SetHandler(e => { });
            });

            builder.AddGroup(group =>
            {
                group
                    .WithName("department")
                    .WithDescription("manage the departments")
                    .AddGroup(builder =>
                    {
                        builder
                            .WithName("users")
                            .WithDescription("manage the users");
                    });

                group.AddCommand(command =>
                {
                    command
                        .WithName("create")
                        .WithDescription("creates a new department")
                        .SetHandler(e => { });
                });
            });


            builder.Validate();
            builder.CommandGroups.Should().HaveCount(1);
            builder.Commands.Should().HaveCount(1);
        }

        [Test]
        public void Should_throw_duplicate_command_because_there_is_two_commands_with_the_same_name()
        {
            var builder = new CommandBuilder();
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
            var builder = new CommandBuilder();
            builder.AddGroup(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users");
            });
            builder.AddGroup(users =>
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
            var builder = new CommandBuilder();
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
            var builder = new CommandBuilder();
            builder.AddGroup(group =>
            {
                group
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
            var builder = new CommandBuilder();
            builder
                .AddGroup(group =>
                {
                    group
                        .WithName("department")
                        .WithDescription("manage the departments")
                        .AddGroup(builder =>
                        {
                            builder
                                .WithName("users")
                                .WithDescription("manage the users");
                        })
                        .AddGroup(builder =>
                        {
                            builder
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
