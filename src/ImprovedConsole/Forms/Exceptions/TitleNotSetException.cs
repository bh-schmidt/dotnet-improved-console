using ImprovedConsole.Forms.Fields;
using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Exceptions
{
    public class TitleNotSetException(string? message) : Exception(message)
    {
        public static void ThrowIfNullOrEmpty([NotNull] string? title)
        {
            if (string.IsNullOrEmpty(title))
                throw new TitleNotSetException($"The title was not set for the field.");
        }

        public static void ThrowIfNull([NotNull] object? obj)
        {
            if (obj is null)
                throw new TitleNotSetException($"The title was not set for the field.");
        }
    }
}
