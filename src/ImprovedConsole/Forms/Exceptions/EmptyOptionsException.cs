using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Exceptions
{
    public class EmptyOptionsException() : Exception("Can't set null or empty options.")
    {
        public static void ThrowIfNullOrEmpty<T>([NotNull] IEnumerable<T> options)
        {
            if (options is null || !options.Any())
                throw new EmptyOptionsException();
        }
    }
}
