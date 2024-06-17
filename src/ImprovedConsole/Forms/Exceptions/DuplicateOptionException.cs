namespace ImprovedConsole.Forms.Exceptions
{
    internal class DuplicateOptionException(object option) : Exception("The options can't be duplicated.")
    {
        public object Option { get; } = option;

        public static void ThrowIfDuplicated<T>(IEnumerable<T> options)
        {
            var invalidOptions = options
                .GroupBy(e => e)
                .Where(e => e.LongCount() > 1)
                .Select(e => e.First())
                .ToArray();

            if (invalidOptions.Length > 0)
                throw new DuplicateOptionException(invalidOptions.First()!);
        }
    }
}
