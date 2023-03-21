namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongField : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Action<long?> OnConfirmEvent = (e) => { };
        private Action OnResetAction = () => { };

        public LongField(
            FormEvents formEvents,
            string title,
            LongFieldOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string Title { get; private set; }
        public LongFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            string? value;
            long? convertedValue;

            do
            {
                formEvents.Reprint();
                value = Read();

                convertedValue = long.TryParse(value, out long parsed) ?
                    parsed :
                    null;
            } while ((Options.Required && convertedValue is null) || (!string.IsNullOrWhiteSpace(value) && convertedValue is null));

            OnConfirmEvent(convertedValue);
            return new LongFieldAnswer(this, convertedValue);
        }

        private string? Read()
        {
            string? line;
            Message.WriteLine(Title);
            line = ConsoleWriter.ReadLine();
            return line;
        }

        public LongField OnConfirm(Action<long?> onConfirm)
        {
            if (onConfirm is null)
                throw new ArgumentNullException(nameof(onConfirm));

            OnConfirmEvent += onConfirm;
            return this;
        }

        public LongField OnReset(Action onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        void IResetable.Reset()
        {
            OnResetAction();
        }
    }
}
