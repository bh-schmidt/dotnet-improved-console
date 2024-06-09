using ImprovedConsole.CommandRunners;
using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.ConsoleMockers;
using System.Text;

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
            builder.AddGroup(users =>
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

            var arguments = new string[] { "users", "delete" };

            var builder = new CommandBuilder();
            builder.AddGroup(users =>
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
                                createExecuted = true;
                            });
                    });
            });

            builder.AddCommand(users =>
            {
                users
                    .WithName("users")
                    .WithDescription("List all the users")
                    .SetHandler(e =>
                    {
                        listExecuted = true;
                    });
            });

            var runner = new SafeCommandRunner(builder);

            runner.Run(arguments);
            createExecuted.Should().BeFalse();
            listExecuted.Should().BeTrue();
        }

        [Test]
        public void Should_throw_command_handler_not_set()
        {
            var arguments = new string[] { "users", "create", "user_01", "123456" };

            var builder = new CommandBuilder();
            builder.AddGroup(users =>
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
@"The handler for the command 'users create' was not set
");
        }

        [Test]
        public void Should_throw_execution_exception()
        {
            var arguments = new string[] { "users", "create" };

            var builder = new CommandBuilder();
            builder.AddGroup(users =>
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

            var runner = new SafeCommandRunner(builder);

            runner.Run(arguments);

            mocker.GetOutput().Should().StartWith(
@"An error ocurred executing the command 'users create'
System.Exception: An error ocurred");
        }

        [Test]
        public void Should_show_the_group_help()
        {
            var builder = new CommandBuilder();
            builder.AddGroup(department =>
            {
                department
                    .WithName("department")
                    .WithDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .SetOptionsName("department-options");

                department.AddGroup(builder =>
                {
                    builder
                        .WithName("users")
                        .WithDescription("Manage the department users");
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
    users      Manage the department users

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
            builder.AddGroup(group =>
            {
                group
                    .WithName("departments")
                    .WithDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .SetOptionsName("department-options");

                group.AddCommand(command =>
                {
                    command
                        .WithName("create")
                        .WithDescription("Creates a new department")
                        .AddParameter("name", "The department name")
                        .AddOption("--manager", "The department manager")
                        .SetOptionsName("options")
                        .SetHandler(c => { });
                });

                group.AddGroup(builder =>
                {
                    builder
                        .WithName("users")
                        .WithDescription("Manage the department users");
                });

                group.AddCommand(update =>
                {
                    update
                        .WithName("update")
                        .WithDescription("Updates the department")
                        .SetHandler(c => { });
                });
            });

            builder.AddCommand(command =>
            {
                command
                    .WithName("departments")
                    .WithDescription("List departments")
                    .AddFlag("-a", "List all departments")
                    .SetOptionsName("options")
                    .SetHandler(c => { });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "departments", "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"usage:
  List departments
    departments [options]

  Manage the departments
    departments [department-options] [command] [command-options]

commands:
    create     Creates a new department
    update     Updates the department
    users      Manage the department users

options:
    -a         List all departments

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
            builder.AddGroup(group =>
            {
                group
                    .WithName("departments")
                    .WithDescription("Manage the departments")
                    .AddFlag("--admin", "Manage the admin departments")
                    .AddOption("--name", "Filter the departments by the name")
                    .AddGroup(usersGroup =>
                    {
                        usersGroup
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

            builder.AddCommand(command =>
            {
                command
                    .WithName("departments")
                    .WithDescription("List departments")
                    .AddFlag("-a", "List all departments")
                    .SetHandler(c => { });
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
            var builder = new CommandBuilder(new CommandBuilderOptions
            {
                CliName = "cli"
            });

            builder.AddGroup(group =>
            {
                group
                    .WithName("departments")
                    .WithDescription("Manage the departments")
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

            builder.AddCommand(command =>
            {
                command
                    .WithName("departments")
                    .WithDescription("List departments")
                    .SetOptionsName("list-options")
                    .AddFlag("-a", "List all departments")
                    .SetHandler(c => { });
            });

            builder.AddDefaultCommand(command =>
            {
                command
                    .WithDescription("Prints data registered")
                    .AddParameter("name", "Name of the data")
                    .AddFlag("-i", "Ignore null data")
                    .SetOptionsName("cli-options")
                    .SetHandler(c => { });
            });

            var runner = new SafeCommandRunner(builder);

            using var mocker = new ConsoleMock();

            var arguments = new string[] { "--help" };
            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
"""
usage:
  Prints data registered
    cli [cli-options] [name]

  Executes the commands
    cli [command] [command-options]

parameters:
    name           Name of the data

commands:
    departments    List departments
    departments    Manage the departments

cli-options:
    -i             Ignore null data

For more information about the commands please run
     cli [command] --help

""");
        }

        [Test]
        public void Should_show_the_help_without_matching_any_command_without_a_default_command()
        {
            var builder = new CommandBuilder(new CommandBuilderOptions
            {
                CliName = "cli"
            });

            builder.AddCommand(run =>
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
  Executes the commands
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
            builder.AddGroup(group =>
            {
                group
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
                group
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
            builder.AddGroup(group =>
            {
                group
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

                group.AddCommand(command =>
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
            builder.AddGroup(group =>
            {
                group
                    .WithName("users")
                    .WithDescription("Manage the users")
                    .AddCommand(builder =>
                    {
                        builder
                            .WithName("create")
                            .WithDescription("Creates a new user");
                    });
            });

            builder.AddGroup(group =>
            {
                group
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
        of type ImprovedConsole.CommandRunners.Commands.CommandGroup

    users
        of type ImprovedConsole.CommandRunners.Commands.CommandGroup
");
        }

        [Test]
        public void Should_throw_command_not_found_when_args_are_empty()
        {
            var arguments = new string[] { };
            var builder = new CommandBuilder();
            builder.AddGroup(group =>
            {
                group
                    .WithName("users")
                    .WithDescription("Manage the users")
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
@"Command not found. Try using -h or --help to list the commands.
");
        }

        [Test]
        public void Should_throw_command_not_found_when_the_group_was_not_found()
        {
            var arguments = new string[] { "departments", "create", "department 1" };

            var builder = new CommandBuilder();
            builder.AddGroup(group =>
            {
                group
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

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"Command not found. Try using -h or --help to list the commands.
");
        }

        [Test]
        public void Should_throw_command_not_found_when_the_group_was_found()
        {
            var arguments = new string[] { "users", "update" };

            var builder = new CommandBuilder();

            builder.AddGroup(group =>
            {
                group
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

            runner.Run(arguments);

            mocker.GetOutput().Should().Be(
@"Command not found. Try using users -h/--help to list the commands.
");
        }
    }
}
