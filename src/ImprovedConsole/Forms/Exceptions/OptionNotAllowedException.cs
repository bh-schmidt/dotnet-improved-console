using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Exceptions
{
    public class OptionNotAllowedException(object? value, IEnumerable<object> options) : Exception("The informed value is not a valid option.")
    {
        public object? Value { get; } = value;
        public IEnumerable<object> Options { get; } = options;

        public static void ThrowIfInvalid<T>(T value, IEnumerable<T> options)
        {
            if (!options.Contains(value))
                throw new OptionNotAllowedException(value, options.Cast<object>());
        }

        public static void ThrowIfInvalid<T>(IEnumerable<T> values, IEnumerable<T> options)
        {
            var set = options.ToHashSet();
            var invalidValues = values
                .Where(e => !set.Contains(e))
                .ToArray();

            if (invalidValues.Length != 0)
                throw new OptionNotAllowedException(invalidValues.First(), set.Cast<object>());
        }
    }
}
