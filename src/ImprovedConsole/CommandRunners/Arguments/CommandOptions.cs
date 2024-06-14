using System.Collections;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class CommandOptions(IEnumerable<ArgumentOption> options) : IEnumerable<ArgumentOption>
    {
        public IEnumerator<ArgumentOption> GetEnumerator()
        {
            return options.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return options.GetEnumerator();
        }

        public ArgumentOption? First(string name) => options.FirstOrDefault(e => e.Option.Name == name);
        public ArgumentOption? Last(string name) => options.LastOrDefault(e => e.Option.Name == name);
        public ArgumentOption[] Get(string name) => options.Where(e => e.Option.Name == name).ToArray();
    }
}
