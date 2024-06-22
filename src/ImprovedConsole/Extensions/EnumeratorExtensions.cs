namespace ImprovedConsole.Extensions
{
    public static class EnumeratorExtensions
    {
        public static IEnumerator<T> Concat<T>(this IEnumerator<T> source, IEnumerable<T> values)
        {
            while (source.MoveNext())
                yield return source.Current;

            foreach (var value in values)
                yield return value;
        }
    }
}
