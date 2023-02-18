using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.Tests.CommandRunners.Commands
{
    public class CommandBuilderTests
    {
        [Test]
        public void Should_register_groups_and_commands()
        {
            var registrator = new InlineCommandBuilder();

            registrator .AddCommand("cache", "Enable or disable the cache", command =>
            {
                command.SetHandler(e => { });
            });

            registrator.AddGroup("department", "manage the departments", group =>
            {
                group.AddGroup("users", "manage the users", builder => { });

                group.AddCommand("create", "creates a new department", command =>
                {
                    command.SetHandler(e => { });
                });
            });


            registrator.Validate();
            registrator.CommandGroups.Should().HaveCount(1);
            registrator.Commands.Should().HaveCount(1);
        }

        [Test]
        public void Should_throw_duplicate_command_because_there_is_two_commands_with_the_same_name()
        {
            var registrator = new InlineCommandBuilder();
            registrator.AddCommand("cache", "Enable or disable the cache");
            registrator.AddCommand("cache", "Enable or disable the cache");

            Assert.Catch<DuplicateCommandException>(() =>
            {
                registrator.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_group_because_there_is_two_command_groups_with_the_same_name()
        {
            var registrator = new InlineCommandBuilder();
            registrator.AddGroup("users", "Manage the users");
            registrator.AddGroup("users", "Manage the users");

            Assert.Catch<DuplicateCommandException>(() =>
            {
                registrator.Validate();
            });
        }

        [Test]
        public void Should_throw_handler_not_set()
        {
            var registrator = new InlineCommandBuilder();
            registrator.AddCommand("cache", "Enable or disable the cache");

            Assert.Catch<HandlerNotSetException>(() =>
            {
                registrator.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_because_there_is_two_commands_with_the_same_name_inside_the_same_group()
        {
            var registrator = new InlineCommandBuilder();
            registrator.AddGroup("department", "manage the departments", group =>
            {
                group.AddCommand("create", "creates a new department", builder => { });
                group.AddCommand("create", "creates a new department", builder => { });
            });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                registrator.Validate();
            });
        }

        [Test]
        public void Should_throw_duplicate_command_group_because_there_is_two_command_groups_with_the_same_name_inside_the_same_group()
        {
            var registrator = new InlineCommandBuilder();
            registrator
                .AddGroup("department", "manage the departments", group =>
                {
                    group.AddGroup("users", "manage the users", builder => { });
                    group.AddGroup("users", "manage the users", builder => { });
                });

            Assert.Catch<DuplicateCommandException>(() =>
            {
                registrator.Validate();
            });
        }
    }
}
