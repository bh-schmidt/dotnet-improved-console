using System.Collections;

namespace ImprovedConsole.CommandRunners.Arguments
{
    public class CommandParameters : IEnumerable<ArgumentParameter>
    {
        private IEnumerable<ArgumentParameter> parameters;

        public CommandParameters(IEnumerable<ArgumentParameter> parameters)
        {
            this.parameters = parameters;
        }

        public IEnumerator<ArgumentParameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        public ArgumentParameter? First(string name) => parameters.FirstOrDefault(e => e.Parameter.Name == name);
        public ArgumentParameter? Last(string name) => parameters.LastOrDefault(e => e.Parameter.Name == name);
        public ArgumentParameter[] Get(string name) => parameters.Where(e => e.Parameter.Name == name).ToArray();
    }
}
