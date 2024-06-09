using ImprovedConsole.CommandRunners.Arguments;

namespace ImprovedConsole.CommandRunners.Handlers
{
    public class AsyncHandler(Func<ExecutionArguments, Task> handler) : IHandler
    {
        public Func<ExecutionArguments, Task> Handle { get; private set; } = handler;
    }
}
