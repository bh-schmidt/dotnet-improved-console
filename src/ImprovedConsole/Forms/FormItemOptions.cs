using ImprovedConsole.Forms.Fields;

namespace ImprovedConsole.Forms
{
    public class FormItemOptions
    {
        public Func<bool> Condition { get; set; } = () => true;
        public DependsOnFields? DependsOn { get; set; }
    }

    public class DependsOnFields
    {
        private HashSet<IField> fields;

        public DependsOnFields(params IField[] fields)
        {
            this.fields = fields.Distinct().ToHashSet();
        }

        public bool Contains(IField field) => fields.Contains(field);
    }
}
