using ImprovedConsole.CommandRunners.Arguments;

namespace ImprovedConsole.CommandRunners.Handlers
{
    public interface ICommandHandler
    {
        Task<int> ExecuteAsync(ExecutionArguments arguments);
    }
}
