namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextField : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Action<string?> OnConfirmEvent = (e) => { };
        private Action OnResetAction = () => { };

        public TextField(
            FormEvents formEvents,
            string title,
            TextFieldOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string Title { get; private set; }
        public TextFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            string? value = Read();

            while (Options.Required && string.IsNullOrEmpty(value))
            {
                formEvents.Reprint();
                value = Read();
            }

            if (string.IsNullOrEmpty(value))
                value = null;

            OnConfirmEvent(value);
            return new TextFieldAnswer(this, value);
        }

        private string? Read()
        {
            string? line;
            Message.WriteLine(Title);
            line = ConsoleWriter.ReadLine();
            return line;
        }

        public TextField OnConfirm(Action<string?> onConfirm)
        {
            if (onConfirm is null)
                throw new ArgumentNullException(nameof(onConfirm));

            OnConfirmEvent += onConfirm;
            return this;
        }

        public TextField OnReset(Action onReset)
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
