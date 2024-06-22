using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    public interface IFieldWrapper
    {
        public Type Type { get; }
    }

    public class FieldWrapper<T>(FormEvents events, T[] options) : IFieldWrapper
    {
        public TextField<T> Field { get; set; } = new(events);
        public T[] Options { get; set; } = options;
        public T First => Options.First();
        public T Last => Options.Last();
        public T? Default => default;
        public Type Type => typeof(T);
    }
}
