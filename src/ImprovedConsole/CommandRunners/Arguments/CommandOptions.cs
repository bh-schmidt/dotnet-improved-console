using ImprovedConsole.CommandRunners.Exceptions;
using System.Collections;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class CommandOptions : IEnumerable<ArgumentOption>
    {
        private IEnumerable<ArgumentOption> options;

        public CommandOptions(IEnumerable<ArgumentOption> options)
        {
            this.options = options;
        }

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
