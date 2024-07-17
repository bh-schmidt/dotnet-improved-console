using ImprovedConsole.Forms.Fields;
using System.Collections;

namespace ImprovedConsole.Forms
{
    public class FormItemDependencies(params IField[] fields) : IEnumerable<IField>
    {
        private readonly HashSet<IField> fields = fields.Distinct().ToHashSet();

        public void Add(IField field)
        {
            if (fields.Contains(field))
                return;

            fields.Add(field);
        }

        internal bool Contains(IField field) => fields.Contains(field);

        public IEnumerator<IField> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fields.GetEnumerator();
        }
    }
}
