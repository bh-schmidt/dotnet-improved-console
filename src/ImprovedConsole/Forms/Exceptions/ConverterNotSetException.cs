using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Exceptions
{
    public class ConverterNotSetException(string? message) : Exception(message)
    {
        public static void ThrowIfNull([NotNull] object? obj)
        {
            if (obj is null)
                throw new ConverterNotSetException($"The converter was not set for the field.");
        }
    }
}
