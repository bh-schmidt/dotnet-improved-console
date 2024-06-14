using ImprovedConsole.CommandRunners.Arguments;

namespace ImprovedConsole.CommandRunners.Handlers
{
    public class CommandHandler(Func<ExecutionArguments, Task<int>> handler) : ICommandHandler
    {
        public Func<ExecutionArguments, Task<int>> Handler { get; private set; } = handler;

        public async Task<int> ExecuteAsync(ExecutionArguments arguments)
        {
            return await Handler(arguments);
        }
    }
}
