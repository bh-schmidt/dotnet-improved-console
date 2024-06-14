namespace ImprovedConsole.CommandRunners.Exceptions
{
    public class InvalidCommandHandlerException(Type type) : Exception($"The command handler informed '{type.Name}' could not be initiated.")
    {
    }
}
