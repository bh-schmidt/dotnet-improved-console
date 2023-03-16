using ImprovedConsole.Forms.Fields;

namespace ImprovedConsole.Forms
{
    public class FormItemOptions
    {
        public Func<bool> Condition { get; set; } = () => true;
        public FormItemDependencies? Dependencies { get; set; }
    }

    public class FormItemDependencies
    {
        private HashSet<IField> fields;

        public FormItemDependencies(params IField[] fields)
        {
            this.fields = fields.Distinct().ToHashSet();
        }

        internal bool Contains(IField field) => fields.Contains(field);
    }
}
