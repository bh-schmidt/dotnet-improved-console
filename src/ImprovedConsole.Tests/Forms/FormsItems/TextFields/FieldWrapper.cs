using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    public interface IFieldWrapper
    {
        public Type Type { get; }
    }

    public class FieldWrapper<T>(FormEvents events, T validValue) : IFieldWrapper
    {
        public TextField<T> Field { get; set; } = new(events);
        public T ValidValue => validValue;
        public T? Default => default;
        public Type Type => typeof(T);
    }
}
