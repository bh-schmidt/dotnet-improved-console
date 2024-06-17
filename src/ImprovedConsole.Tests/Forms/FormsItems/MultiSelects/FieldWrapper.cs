using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.MultiSelects;

namespace ImprovedConsole.Tests.Forms.FormsItems.MultiSelects
{
    public interface IFieldWrapper
    {
        public Type Type { get; }
    }

    public class FieldWrapper<T>(FormEvents events, T[] options) : IFieldWrapper
    {
        public MultiSelect<T> Field { get; set; } = new MultiSelect<T>(events);
        public T[] Options { get; set; } = options;
        public T First => Options.First();
        public T Last => Options.Last();
        public T? Default => default;
        public Type Type => typeof(T);
    }
}
