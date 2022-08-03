using FluentAssertions;
using FluentAssertions.Primitives;

namespace ImprovedConsole.Tests
{
    public class ConsoleHelper : IDisposable
    {
        private StringWriter stringWriter;
        private StringReader? stringReader;

        public ConsoleHelper()
        {
            stringWriter = new StringWriter();
            ConsoleWriter.SetOut(stringWriter);
        }

        public ConsoleHelper Input(string value)
        {
            stringReader?.Dispose();
            stringReader = new StringReader(value);
            ConsoleWriter.SetIn(stringReader);

            return this;
        }

        public StringAssertions Should() => stringWriter.ToString().Should();

        public void Dispose()
        {
            stringWriter.Dispose();
            stringReader?.Dispose();
        }
    }
}
