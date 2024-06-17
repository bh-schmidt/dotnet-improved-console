using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Exceptions
{
    public class OptionsNotSetException() : Exception("The options were not set.")
    {
        public static void ThrowIfNull([NotNull] object? obj)
        {
            if (obj is null)
                throw new OptionsNotSetException();
        }
    }
}
