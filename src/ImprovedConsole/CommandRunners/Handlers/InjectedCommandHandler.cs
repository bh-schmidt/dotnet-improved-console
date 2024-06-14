using ImprovedConsole.CommandRunners.Arguments;
using ImprovedConsole.CommandRunners.Exceptions;

namespace ImprovedConsole.CommandRunners.Handlers
{
    public class InjectedCommandHandler(Type type) : ICommandHandler
    {
        private ICommandHandler GetInstance(IServiceProvider? serviceProvider)
        {
            if (serviceProvider is not null)
            {
                var handler = serviceProvider.GetService(type);
                if (handler is not null)
                {
                    return (ICommandHandler)handler;
                }
            }

            object instance = Activator.CreateInstance(type) ?? throw new InvalidCommandHandlerException(type);
            return (ICommandHandler)instance;
        }

        public async Task<int> ExecuteAsync(ExecutionArguments arguments)
        {
            var handler = GetInstance(arguments.ServiceProvider);
            return await handler!.ExecuteAsync(arguments);
        }
    }
}
