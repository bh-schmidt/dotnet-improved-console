using ImprovedConsole.CommandRunners.Commands;
using ImprovedConsole.Extensions;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class NameNotSetException(Command command) : Exception(GetMessage(command))
    {
        public Command Command { get; } = command;

        private static string GetMessage(Command command)
        {
            var tree = command.GetCommandTreeAsString();
            if (tree.IsNullOrEmpty())
            {
                return $"There is a command in the root of command builder that has no name";
            }

            return $"There is a command without a name inside the command '{tree}'";
        }
    }
}
