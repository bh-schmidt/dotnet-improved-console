using ImprovedConsole.Extensions;
using System.Linq;

namespace ImprovedConsole.ConsoleMockers
{
    public class ConsoleMockerSetup(MockerInstance instance)
    {
        private readonly MockerInstance instance = instance;

        public ConsoleMockerSetup ReadKey(params ConsoleKey[] keys)
        {
            return ReadKey(keys.AsEnumerable());
        }

        public ConsoleMockerSetup ReadKey(IEnumerable<ConsoleKey> keys)
        {
            var keyInfos = keys
                .Select(key => new ConsoleKeyInfo('\0', key, false, false, false));

            if (instance.ReadKeyEnumerator is not null)
            {
                instance.ReadKeyEnumerator = instance.ReadKeyEnumerator
                    .Concat(keyInfos);
                return this;
            }

            instance.ReadKeyEnumerator = keyInfos.GetEnumerator();
            return this;
        }

        public ConsoleMockerSetup ReadLine(params string[] lines)
        {
            return ReadLine(lines.AsEnumerable());
        }

        public ConsoleMockerSetup ReadLine(IEnumerable<string> lines)
        {
            if (instance.ReadLineEnumerator is not null)
            {
                instance.ReadLineEnumerator = instance.ReadLineEnumerator
                    .Concat(lines);
                return this;
            }

            instance.ReadLineEnumerator = lines.GetEnumerator();
            return this;
        }
    }
}
