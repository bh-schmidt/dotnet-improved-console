using ImprovedConsole.Forms.Fields.TextOptions;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Tests.Forms.FormsItems.SingleSelects
{
    public interface IFieldWrapper
    {
        public Type Type { get; }
    }

    public class FieldWrapper<T>(FormEvents events, T[] options) : IFieldWrapper
    {
        public SingleSelect<T> Field { get; set; } = new SingleSelect<T>(events);
        public T[] Options { get; set; } = options;
        public T First => Options.First();
        public T Last => Options.Last();
        public T? Default => default;
        public Type Type => typeof(T);
    }
}
