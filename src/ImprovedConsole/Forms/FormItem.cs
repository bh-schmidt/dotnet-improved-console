using ImprovedConsole.Forms.Fields;
using ImprovedConsole.Forms.Fields.MultiSelects;
using ImprovedConsole.Forms.Fields.SingleSelects;
using ImprovedConsole.Forms.Fields.TextFields;
using ImprovedConsole.Forms.Fields.TextOptions;
using System.Text;

namespace ImprovedConsole.Forms
{
    public class FormItem(FormEvents formEvents, FormItemOptions itemOptions)
    {
        public object Id { get; set; } = Guid.NewGuid();
        public FormItemOptions Options { get; } = itemOptions ?? throw new ArgumentNullException(nameof(itemOptions));
        public IField? Field { get; private set; }

        public bool Finished => Field?.Finished ?? false;

        public TextField<string> TextField()
        {
            TextField<string> field = new(formEvents);
            Field = field;
            return field;
        }

        public TextField<TField> TextField<TField>()
        {
            TextField<TField> field = new(formEvents);
            Field = field;
            return field;
        }

        public TextOption<string> TextOption()
        {
            TextOption<string> field = new(formEvents);
            Field = field;
            return field;
        }

        public TextOption<TFieldType> TextOption<TFieldType>()
        {
            TextOption<TFieldType> field = new(formEvents);
            Field = field;
            return field;
        }

        public SingleSelect<string> SingleSelect()
        {
            SingleSelect<string> field = new(formEvents);
            Field = field;
            return field;
        }

        public SingleSelect<TFieldType> SingleSelect<TFieldType>()
        {
            SingleSelect<TFieldType> field = new(formEvents);

            Field = field;
            return field;
        }

        public MultiSelect<string> MultiSelect()
        {
            MultiSelect<string> field = new(formEvents);
            Field = field;
            return field;
        }

        public MultiSelect<TFieldType> MultiSelect<TFieldType>()
        {
            MultiSelect<TFieldType> field = new(formEvents);

            Field = field;
            return field;
        }

        internal bool Run()
        {
            Validate();
            var oldAnswer = Field!.Answer;
            var answer = Field!.Run();

            return answer.Equals(oldAnswer);
        }

        internal StringBuilder GetFormattedAnswer(int leftSpacing, FormOptions options)
        {
            return Field!.Answer!.GetFormattedAnswer(leftSpacing, options);
        }

        internal void Validate()
        {
            if (Field is null)
                throw new ArgumentNullException(nameof(Field));
        }

        internal void Edit()
        {
            Field?.SetEdition();
        }

        internal void Reset()
        {
            Field?.Reset();
        }
    }
}
