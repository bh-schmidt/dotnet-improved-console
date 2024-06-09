using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Extensions
{
    internal static class EnuberableExtensions
    {
        internal static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? value)
        {
            return value is null || !value.Any();
        }
    }
}
