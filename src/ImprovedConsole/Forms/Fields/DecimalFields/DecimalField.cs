namespace ImprovedConsole.Forms.Fields.DecimalFields
{
    public class DecimalField : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Action<decimal?> OnConfirmEvent = (e) => { };
        private Action OnResetAction = () => { };

        public DecimalField(
            FormEvents formEvents,
            string title,
            DecimalFieldOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string Title { get; private set; }
        public DecimalFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            string? value;
            decimal? convertedValue;

            do
            {
                formEvents.Reprint();
                value = Read();

                convertedValue = decimal.TryParse(value, out decimal parsed) ?
                    parsed :
                    null;
            } while ((Options.Required && convertedValue is null) || (!string.IsNullOrWhiteSpace(value) && convertedValue is null));

            OnConfirmEvent(convertedValue);
            return new DecimalFieldAnswer(this, convertedValue);
        }

        private string? Read()
        {
            string? line;
            Message.WriteLine(Title);
            line = ConsoleWriter.ReadLine();
            return line;
        }

        public DecimalField OnConfirm(Action<decimal?> onConfirm)
        {
            if (onConfirm is null)
                throw new ArgumentNullException(nameof(onConfirm));

            OnConfirmEvent += onConfirm;
            return this;
        }

        public DecimalField OnReset(Action onReset)
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
