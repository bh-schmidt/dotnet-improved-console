using ImprovedConsole.Forms.Fields;
using System.Collections;

namespace ImprovedConsole.Forms
{
    public class FormItemOptions
    {
        public Func<bool> Condition { get; set; } = () => true;
        public FormItemDependencies? Dependencies { get; set; }
    }

    public class FormItemDependencies : IEnumerable<IField>
    {
        private HashSet<IField> fields;

        public FormItemDependencies(params IField[] fields)
        {
            this.fields = fields.Distinct().ToHashSet();
        }

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
