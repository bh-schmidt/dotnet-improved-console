using ImprovedConsole.CommandRunners.Arguments;

namespace ImprovedConsole.CommandRunners.Handlers
{
    public class SyncHandler(Action<ExecutionArguments> handler) : IHandler
    {
        public Action<ExecutionArguments> Handle { get; private set; } = handler;
    }
}
