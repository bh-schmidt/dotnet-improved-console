using ImprovedConsole.Forms.Fields;
using ImprovedConsole.Forms.Fields.DecimalFields;
using ImprovedConsole.Forms.Fields.LongFields;
using ImprovedConsole.Forms.Fields.MultiSelects;
using ImprovedConsole.Forms.Fields.SingleSelects;
using ImprovedConsole.Forms.Fields.TextFields;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Forms
{
    public class FormItem
    {
        private readonly FormEvents formEvents;
        private IFieldAnswer? answer;

        public FormItem(FormEvents formEvents, FormItemOptions itemOptions)
        {
            this.formEvents = formEvents;
            Options = itemOptions ?? throw new ArgumentNullException(nameof(itemOptions));
        }

        public FormItemOptions Options { get; }
        public bool Finished { get; private set; }
        public IField? Field { get; private set; }
        public Guid ExecutionId { get; private set; } = new Guid();

        public TextField TextField(string title, TextFieldOptions? options = null)
        {
            var field = new TextField(formEvents, title, options ?? new TextFieldOptions());
            Field = field;
            return field;
        }

        public LongField LongField(string title, LongFieldOptions? options = null)
        {
            var field = new LongField(formEvents, title, options ?? new LongFieldOptions());
            Field = field;
            return field;
        }

        public DecimalField DecimalField(string title, DecimalFieldOptions? options = null)
        {
            var field = new DecimalField(formEvents, title, options ?? new DecimalFieldOptions());
            Field = field;
            return field;
        }

        public TextOption TextOption(string title, IEnumerable<string> possibilities, TextOptionOptions? options = null)
        {
            var field = new TextOption(formEvents, title, possibilities, options ?? new TextOptionOptions());
            Field = field;
            return field;
        }

        public TextOption TextOption(string title, Func<IEnumerable<string>> getPossibilities, TextOptionOptions? options = null)
        {
            var field = new TextOption(formEvents, title, getPossibilities, options ?? new TextOptionOptions());
            Field = field;
            return field;
        }

        public SingleSelect SingleSelect(string title, IEnumerable<string> possibilities, SingleSelectOptions? options = null)
        {
            var field = new SingleSelect(formEvents, title, possibilities, options ?? new SingleSelectOptions());
            Field = field;
            return field;
        }

        public SingleSelect SingleSelect(string title, Func<IEnumerable<string>> getPossibilities, SingleSelectOptions? options = null)
        {
            var field = new SingleSelect(
                formEvents,
                title,
                () => getPossibilities()?.Select(e => new Fields.SingleSelects.PossibilityItem(e))!,
                options ?? new SingleSelectOptions());

            Field = field;
            return field;
        }

        public MultiSelect MultiSelect(string title, IEnumerable<string> possibilities, MultiSelectOptions? options = null)
        {
            var field = new MultiSelect(formEvents, title, possibilities, options ?? new MultiSelectOptions());
            Field = field;
            return field;
        }

        public MultiSelect MultiSelect(string title, Func<IEnumerable<string>> getPossibilities, MultiSelectOptions? options = null)
        {
            var field = new MultiSelect(
                formEvents,
                title,
                () => getPossibilities()?.Select(e => new Fields.MultiSelects.PossibilityItem(e))!,
                options ?? new MultiSelectOptions());

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
            if (Field is IResetable resetable)
                resetable.Reset(answer);

            answer = null;
            Finished = false;
            ExecutionId = Guid.NewGuid();
        }
    }
}
