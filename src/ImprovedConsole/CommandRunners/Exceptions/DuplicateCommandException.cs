using ImprovedConsole.CommandRunners.Commands;
using System.Text;

namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class DuplicateCommandException(IEnumerable<Command> commands) : Exception(GetMessage(commands))
    {
        public IEnumerable<Command> Commands { get; } = commands;


        private static string GetMessage(IEnumerable<Command> commands)
        {
            StringBuilder builder = new StringBuilder()
                .Append("The following commands are facing conflict");

            int lastIndex = commands.Count() - 1;
            int index = 0;

            foreach (Command command in commands)
            {
                builder
                    .AppendLine()
                    .Append(' ', 4)
                    .Append(command.GetCommandTreeAsStringBuilder())
                    .AppendLine()
                    .Append(' ', 8)
                    .Append("of type ")
                    .Append(command.GetType().FullName);

                if (index < lastIndex)
                    builder.AppendLine();

                index++;
            }

            return builder.ToString();
        }

    }
}
