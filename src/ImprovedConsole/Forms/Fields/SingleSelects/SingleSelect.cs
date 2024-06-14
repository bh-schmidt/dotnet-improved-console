namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelect : IField, IResettable
    {
        private readonly Func<PossibilityItem[]> getPossibilities;
        internal Action<PossibilityItem?> OnConfirmAction = (e) => { };
        internal Action<PossibilityItem> OnChangeAction = (e) => { };
        internal Action<PossibilityItem?> OnResetAction = (e) => { };
        private InitialValue<string?>? initialValue;

        public SingleSelect(
            string title,
            Func<IEnumerable<PossibilityItem>> getPossibilities,
            SingleSelectOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            ArgumentNullException.ThrowIfNull(getPossibilities);

            Title = title;
            Options = options ?? new SingleSelectOptions();
            OnChangeAction = value => { };
            OnConfirmAction = values => { };
            Writer = new Writer(this);
            KeyHandler = new KeyHandler(this);

            this.getPossibilities = () =>
            {
                IEnumerable<PossibilityItem> possibilities = getPossibilities();
                if (!possibilities.Any())
                    throw new ArgumentException($"'{nameof(possibilities)}' cannot be null or empty.", nameof(title));

                return possibilities.ToArray();
            };
        }

        public SingleSelect(
            string title,
            IEnumerable<string> possibilities,
            SingleSelectOptions options)
            : this(
                  title,
                  () => possibilities?.Select(e => new PossibilityItem(e))!,
                  options)
        {
        }

        internal Writer Writer { get; }
        internal KeyHandler KeyHandler { get; }
        internal SingleSelectErrorEnum? Error { get; set; }
        public string Title { get; private set; }
        public PossibilityItem[] Possibilities { get; private set; } = null!;
        public SingleSelectOptions Options { get; }

        public IFieldAnswer Run()
        {
            Possibilities = getPossibilities();

            if (ReturnInitialValue())
            {
                var answer = new SingleSelectAnswer(this, Possibilities.FirstOrDefault(e => e?.Value == initialValue?.Value));
                initialValue = null;
                return answer;
            }

            bool visible = ConsoleWriter.GetCursorVisibility();

            try
            {
                return RunInternal();
            }
            finally
            {
                ConsoleWriter.SetCursorVisibility(visible);
            }
        }

        private SingleSelectAnswer RunInternal()
        {
            ConsoleWriter.SetCursorVisibility(false);

            int currentIndex = 0;

            Writer.Print();
            (_, int Top) = ConsoleWriter.GetCursorPosition();
            int errorLine = Top;

            SingleSelectAnswer? answer = null;

            while (answer is null)
            {
                Writer.PrintError(ref errorLine);

                ConsoleKeyInfo key = ConsoleWriter.ReadKey(true);

                KeyHandler.HandleKey(ref currentIndex, key);
                answer = KeyHandler.HandleEnter(key);
            }

            return answer;
        }

        public SingleSelect WithValue(string? initialValue)
        {
            this.initialValue = new InitialValue<string?>(initialValue);
            return this;
        }

        public SingleSelect OnConfirm(Action<PossibilityItem?> onConfirm)
        {
            OnConfirmAction += onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));
            return this;
        }

        public SingleSelect OnChange(Action<PossibilityItem> onChange)
        {
            OnChangeAction += onChange ?? throw new ArgumentNullException(nameof(onChange));
            return this;
        }

        public SingleSelect OnReset(Action<PossibilityItem?> onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetAction(null);
                return;
            }

            if (answer is SingleSelectAnswer a)
            {
                OnResetAction(a.Selection);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }

        private bool ReturnInitialValue()
        {
            if (initialValue is null)
                return false;

            if (Options.Required && initialValue.Value is null)
                return false;

            return initialValue.Value is null || Possibilities.Any(e => e.Value == initialValue.Value);
        }
    }
}
