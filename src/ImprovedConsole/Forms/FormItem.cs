using ImprovedConsole.Forms.Fields;
using ImprovedConsole.Forms.Fields.MultiSelects;
using ImprovedConsole.Forms.Fields.SingleSelects;
using ImprovedConsole.Forms.Fields.TextFields;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Forms
{
    public class FormItem(FormEvents formEvents, FormItemOptions itemOptions)
    {
        private IFieldAnswer? answer;

        public FormItemOptions Options { get; } = itemOptions ?? throw new ArgumentNullException(nameof(itemOptions));
        public bool Finished { get; private set; }
        public IField? Field { get; private set; }
        public Guid ExecutionId { get; private set; } = new Guid();

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

        internal void Run()
        {
            Validate();
            answer = Field!.Run();
            Finished = true;
        }

        internal string GetFormattedAnswer(FormOptions options)
        {
            if (!Finished || answer is null)
                throw new Exception();

            return answer.GetFormattedAnswer(options);
        }

        internal void Validate()
        {
            if (Field is null)
                throw new ArgumentNullException(nameof(Field));
        }

        internal void Reset()
        {
            if (Field is IResettable resettable)
                resettable.Reset(answer);

            answer = null;
            Finished = false;
            ExecutionId = Guid.NewGuid();
        }
    }
}
