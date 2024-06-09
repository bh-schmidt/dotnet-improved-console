using ImprovedConsole.CommandRunners;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.ConsoleMockers;

namespace CommandRunner.Tests.CommandRunnerTests.SafeRunnerTests
{
    public class SafeCommandRunnerTests
    {
        [Test]
        public void Should_prepare_the_arguments()
        {
            var executed = false;

            var arguments = new string[] { "users", "create", "user_01", "123456", "--admin", "ignored param", };

            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .AddFlag("--admin", "Put the user at admin group")
                            .AddParameter("username", "The name of the user")
                            .AddParameter("password", "The password of the user")
                            .SetHandler(args =>
                            {
                                args.Options.Where(e => e.Option.Name == "--admin" && e.Value == "--admin").Should().HaveCount(1);
                                args.Parameters.Where(e => e.Parameter!.Name == "username" && e.Value == "user_01").Should().HaveCount(1);
                                args.Parameters.Where(e => e.Parameter!.Name == "password" && e.Value == "123456").Should().HaveCount(1);
                                args.Parameters.Count().Should().Be(2);
                                args.Args.Should().BeEquivalentTo(arguments);

                                executed = true;
                            });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            runner.Run(arguments);
            executed.Should().BeTrue();
        }

        [Test]
        public void Should_list_the_users_because_the_commmand_group_not_matched()
        {
            var listExecuted = false;
            var createExecuted = false;

            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("List all the users")
                    .SetHandler(e =>
                    {
                        listExecuted = true;
                    })
                    .WithGroupDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetHandler(args =>
                            {
                                createExecuted = true;
                            });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            var args = new string[] { "users", "delete" };
            runner.Run(args);
            createExecuted.Should().BeFalse();
            listExecuted.Should().BeTrue();
        }

        [Test]
        public void Should_throw_command_handler_not_set()
        {
            var arguments = new string[] { "users", "create", "user_01", "123456" };

            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    });
            });

            using var mocker = new ConsoleMock();

            var runner = new SafeCommandRunner(builder);
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"The command 'users create' should have either a handler or sub-commands.
");
        }

        [Test]
        public void Should_throw_execution_exception()
        {
            var arguments = new string[] { "users", "create" };

            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetHandler(args =>
                            {
                                throw new Exception("An error ocurred");
                            });
                    });
            });

            using var mocker = new ConsoleMock();

            var runner = new SafeCommandRunner(
                builder,
                new()
                {
                    ExposeExceptionsOnConsole = true
                });

            runner.Run(arguments);

            mocker.GetOutput().Should().StartWith(
@"An error ocurred executing the command 'users create'
System.Exception: An error ocurred");
        }

        [Test]
        public void Should_show_the_group_help()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(department =>
            {
                department
                    .WithName("department")
                    .WithGroupDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .SetOptionsName("department-options");

                department.AddCommand(users =>
                {
                    users
                        .WithName("users")
                        .WithDescription("List the department users")
                        .SetHandler(args => { });
                });

                department.AddCommand(create =>
                {
                    create
                        .WithName("create")
                        .WithDescription("Creates a new department")
                        .SetHandler(c => { });
                });

                department.AddCommand(update =>
                {
                    update
                        .WithName("update")
                        .WithDescription("Updates the department")
                        .SetHandler(c => { });
                });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "department", "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"usage:
  Manage the departments
    department [department-options] [command]

commands:
    create     Creates a new department
    update     Updates the department
    users      List the department users

department-options:
    --admin    Manage the admin departments

For more information about the commands please run
    department [command] --help
");
        }

        [Test]
        public void Should_show_the_help_for_commands_having_a_group_matched()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(departments =>
            {
                departments
                    .WithName("departments")
                    .WithDescription("List departments")
                    .SetHandler(c => { })
                    .WithGroupDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .SetOptionsName("department-options");

                departments.AddCommand(command =>
                {
                    command
                        .WithName("create")
                        .WithDescription("Creates a new department")
                        .AddParameter("name", "The department name")
                        .AddOption("--manager", "The department manager")
                        .SetOptionsName("options")
                        .SetHandler(c => { });
                });

                departments.AddCommand(users =>
                {
                    users
                        .WithName("users")
                        .WithDescription("List the department users")
                        .SetHandler(args => { });
                });

                departments.AddCommand(update =>
                {
                    update
                        .WithName("update")
                        .WithDescription("Updates the department")
                        .SetHandler(c => { });
                });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "departments", "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"usage:
  List departments
    departments [department-options]

  Manage the departments
    departments [department-options] [command] [command-options]

commands:
    create     Creates a new department
    update     Updates the department
    users      List the department users

department-options:
    --admin    Manage the admin departments

For more information about the commands please run
    departments [command] --help
");
        }

        [Test]
        public void Should_show_the_help_for_command_inside_group()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(departments =>
            {
                departments
                    .WithName("departments")
                    .WithDescription("List departments")
                    .SetHandler(args =>
                    {
                    })
                    .WithGroupDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .AddOption("--name", "Filter the departments by the name")
                    .AddCommand(users =>
                    {
                        users
                            .WithName("users")
                            .WithDescription("Manage the department users")
                            .AddOption("--role", "Filter or sets the user role")
                            .AddCommand(command =>
                            {
                                command
                                    .WithName("create")
                                    .WithDescription("Creates a new user at department")
                                    .AddParameter("name", "The user name")
                                    .AddOption("--start-date", "The date that the user will start at department")
                                    .SetHandler(c => { });
                            });
                    })
                    .AddCommand(command =>
                    {
                        command
                            .WithName("create")
                            .WithDescription("Creates a new department")
                            .AddParameter("name", "The department name")
                            .AddOption("--manager", "The department manager")
                            .SetHandler(c => { });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "departments", "users", "create", "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"usage:
  Creates a new user at department
    departments [departments-options] users [users-options] create [create-options] [name]

parameters:
    name            The user name

departments-options:
    --admin         Manage the admin departments
    --name          Filter the departments by the name

users-options:
    --role          Filter or sets the user role

create-options:
    --start-date    The date that the user will start at department
");
        }

        [Test]
        public void Should_show_the_help_without_matching_any_command_with_default_command()
        {
            var builder = new CommandBuilder();

            builder
                .WithName("cli")
                .AddCommand(departments =>
                {
                    departments
                        .WithName("departments")
                        .WithDescription("List departments")
                        .WithGroupDescription("Manage the departments")
                        .SetHandler(args =>
                        {
                        })
                        .AddFlag("--admin", "Manage the admin departments")
                        .AddOption("--name", "Filter the departments by the name")
                        .AddCommand(command =>
                        {
                            command
                                .WithName("create")
                                .WithDescription("Creates a new department")
                                .AddParameter("name", "The department name")
                                .AddOption("--manager", "The department manager")
                                .SetHandler(c => { });
                        });
                });

            builder
                .WithDescription("Prints data registered")
                .AddParameter("name", "Name of the data")
                .SetOptionsName("cli-options")
                .SetHandler(c => { });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
"""
usage:
  Prints data registered
    cli [name]

  Executes the sub-commands
    cli [command] [command-options]

parameters:
    name           Name of the data

commands:
    departments    List departments
                   Manage the departments

For more information about the commands please run
    cli [command] --help

""");
        }

        [Test]
        public void Should_show_the_help_without_matching_any_command_without_a_default_command()
        {
            var builder = new CommandBuilder();

            builder
                .WithName("cli")
                .AddCommand(run =>
                {
                    run
                        .WithName("departments")
                        .WithDescription("List departments")
                        .AddParameter("json-template", "Json file containing all the settings")
                        .SetOptionsName("list-options")
                        .AddFlag("-a", "List all departments")
                        .SetHandler(c => { });
                });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
"""
usage:
  Executes the sub-commands
    cli [command] [command-options]

commands:
    departments    List departments

For more information about the commands please run
    cli [command] --help

""");
        }

        [Test]
        public void Should_show_the_command_help()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(command =>
                    {
                        command
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .AddFlag("--admin", "Put the user at admin group")
                            .AddFlag("--read-only", "Sets the user to read-only mode. Doesn't work with --admin")
                            .AddParameter("username", "The name of the user")
                            .AddParameter("password", "The password of the user")
                            .SetHandler(c => { });
                    });
                users
                    .AddCommand(update =>
                    {
                        update
                            .WithName("update")
                            .WithDescription("Updates the user")
                            .SetHandler(c => { });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "users", "create", "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"usage:
  Creates a new user
    users create [create-options] [username] [password]

parameters:
    username       The name of the user
    password       The password of the user

create-options:
    --admin        Put the user at admin group
    --read-only    Sets the user to read-only mode. Doesn't work with --admin
");
        }

        [Test]
        public void Should_throw_duplicate_command_exception()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(command =>
                    {
                        command
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .AddFlag("--admin", "Put the user at admin group")
                            .AddParameter("username", "The name of the user")
                            .AddParameter("password", "The password of the user");
                    });

                users.AddCommand(command =>
                {
                    command
                        .WithName("create")
                        .WithDescription("Creates a new user")
                        .AddParameter("username", "The name of the user");
                });
            });

            var arguments = new string[] { "users", "create", "user_01", "123456", "--admin", "ignored param", };

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"The following commands are facing conflict
    users create
        of type ImprovedConsole.CommandRunners.Commands.Command

    users create
        of type ImprovedConsole.CommandRunners.Commands.Command
");
        }

        [Test]
        public void Should_throw_duplicate_command_group_exception()
        {
            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    });
            });

            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("update")
                            .WithDescription("Updates the user");
                    });
            });

            var arguments = new string[] { "users", "delete", "user_01", "123456", "--admin", "ignored param", };

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"The following commands are facing conflict
    users
        of type ImprovedConsole.CommandRunners.Commands.Command

    users
        of type ImprovedConsole.CommandRunners.Commands.Command
");
        }

        [Test]
        public void Should_throw_command_not_found_when_args_are_empty()
        {
            var arguments = new string[] { };
            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithGroupDescription("Manage the users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"The command was not found.

usage:
  Executes the sub-commands
    [command]

commands:
    users    Manage the users

For more information about the commands please run
    [command] --help
");
        }

        [Test]
        public void Should_throw_command_not_found_when_the_group_was_not_found()
        {
            var arguments = new string[] { "departments", "create", "department 1" };

            var builder = new CommandBuilder();
            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithGroupDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetHandler(c => { });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"The command was not found.

usage:
  Executes the sub-commands
    [command]

commands:
    users    Manage the users

For more information about the commands please run
    [command] --help
");
        }

        [Test]
        public void Should_throw_command_not_found_when_the_group_was_found()
        {
            var builder = new CommandBuilder();

            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetHandler(c => { });
                    });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "users", "update" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"Wrong command usage.

usage:
  Executes the sub-commands
    users [command]

commands:
    create    Creates a new user

For more information about the commands please run
    users [command] --help
");
        }

        [Test]
        [TestCase([true], Description = "When calling the async method.")]
        [TestCase([false], Description = "When calling the sync method.")]
        public async Task Sync_and_async_executions_should_have_the_same_result(bool callAsync)
        {
            bool asyncCreated = false;
            bool syncCreated = false;

            var asyncBuilder = new CommandBuilder();
            var syncBuilder = new CommandBuilder();

            asyncBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetAsyncHandler(c =>
                            {
                                asyncCreated = true;
                                return Task.CompletedTask;
                            });
                    });
            });

            syncBuilder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(create =>
                    {
                        create
                            .WithName("create")
                            .WithDescription("Creates a new user")
                            .SetHandler(c => { syncCreated = true; });
                    });
            });

            var asyncRunner = new SafeCommandRunner(asyncBuilder);
            var syncRunner = new SafeCommandRunner(syncBuilder);

            var mocker = new ConsoleMock();
            string[] args = ["users", "create"];

            var asyncResult = callAsync ?
                await asyncRunner.RunAsync(args) :
                asyncRunner.Run(args);

            var asyncOutput = mocker.GetOutput();

            mocker.Dispose();
            mocker = new ConsoleMock();

            var syncResult = callAsync ?
                await syncRunner.RunAsync(args) :
                syncRunner.Run(args);

            var syncOutput = mocker.GetOutput();

            asyncCreated.Should().BeTrue();
            syncCreated.Should().BeTrue();

            asyncResult.Should().BeTrue();
            syncResult.Should().BeTrue();

            var expectedOutput = string.Empty;
            asyncOutput.Should().Be(expectedOutput);
            syncOutput.Should().Be(expectedOutput);
        }
    }
}
