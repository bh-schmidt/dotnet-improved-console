using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Tests.Forms.FormsItems.OptionSelectors
{
    public interface IFieldWrapper
    {
        public Type Type { get; }
    }

    public class FieldWrapper<T>(FormEvents events, T[] options) : IFieldWrapper
    {
        public TextOption<T> Field { get; set; } = new TextOption<T>(events);
        public IEnumerable<T> Options { get; set; } = options;
        public T First => Options.First();
        public T Last => Options.Last();
        public T? Default => default;
        public Type Type => typeof(T);
    }
}
