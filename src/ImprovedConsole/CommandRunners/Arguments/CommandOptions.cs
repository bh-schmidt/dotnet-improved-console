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

        public bool Contains(string name) => options.Any(e => e.Option.Name == name);

        public ArgumentOption? this[string name] => options.LastOrDefault(e => e.Option.Name == name);
    }
}
