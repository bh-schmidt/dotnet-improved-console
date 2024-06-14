namespace ImprovedConsole.ConsoleMockers
{
    public class ConsoleMockerSetup(MockerInstance instance)
    {
        private readonly MockerInstance instance = instance;

        public ConsoleMockerSetup ReadKeyReturns(params ConsoleKey[] keys)
        {
            return ReadKeyReturns(keys.AsEnumerable());
        }

        public ConsoleMockerSetup ReadKeyReturns(IEnumerable<ConsoleKey> keys)
        {
            instance.ReadKeyEnumerator = keys
                .Select(key => new ConsoleKeyInfo('\0', key, false, false, false))
                .GetEnumerator();
            return this;
        }

        public ConsoleMockerSetup ReadLineReturns(params string[] lines)
        {
            return ReadLineReturns(lines.AsEnumerable());
        }

        public ConsoleMockerSetup ReadLineReturns(IEnumerable<string> lines)
        {
            instance.ReadLineEnumerator = lines
                .GetEnumerator();
            return this;
        }
    }
}
