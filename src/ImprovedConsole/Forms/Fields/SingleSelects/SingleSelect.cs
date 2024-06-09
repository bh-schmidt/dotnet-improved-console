namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelect : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Func<PossibilityItem[]> getPossibilities { get; }

        public SingleSelect(
            FormEvents formEvents,
            string title,
            Func<IEnumerable<PossibilityItem>> getPossibilities,
            SingleSelectOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            if (getPossibilities is null)
                throw new ArgumentNullException(nameof(getPossibilities));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? new SingleSelectOptions();
            OnChangeAction = value => { };
            OnConfirmAction = values => { };
            Writer = new Writer(this);
            KeyHandler = new KeyHandler(this, Writer);

            this.getPossibilities = () =>
            {
                var possibilities = getPossibilities();
                if (!possibilities.Any())
                    throw new ArgumentException($"'{nameof(possibilities)}' cannot be null or empty.", nameof(title));

                return possibilities.ToArray();
            };
        }

        public SingleSelect(
            FormEvents formEvents,
            string title,
            IEnumerable<string> possibilities,
            SingleSelectOptions options)
            : this(
                  formEvents,
                  title,
                  () => possibilities?.Select(e => new PossibilityItem(e))!,
                  options)
        {
        }

        internal Writer Writer { get; }
        internal KeyHandler KeyHandler { get; }
        internal SingleSelectErrorEnum? Error { get; set; }
        internal Action<PossibilityItem?> OnConfirmAction = (e) => { };
        internal Action<PossibilityItem> OnChangeAction = (e) => { };
        internal Action<PossibilityItem?> OnResetAction = (e) => { };
        public string Title { get; private set; }
        public PossibilityItem[] Possibilities { get; private set; } = null!;
        public SingleSelectOptions Options { get; }

        public IFieldAnswer Run()
        {
            Possibilities = getPossibilities();
            var visible = ConsoleWriter.GetCursorVisibility();

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

            var currentIndex = 0;

            Writer.Print();

            var position = ConsoleWriter.GetCursorPosition();
            var errorLine = position.Top;

            SingleSelectAnswer? answer = null;

            while (answer is null)
            {
                Writer.PrintError(ref errorLine);

                var key = ConsoleWriter.ReadKey(true);

                KeyHandler.HandleKey(ref currentIndex, key);
                answer = KeyHandler.HandleEnter(key);
            }

            return answer;
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

        void IResetable.Reset(IFieldAnswer? answer)
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
    }
}
