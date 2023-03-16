namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelect : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Func<PossibilityItem[]> getPossibilities { get; }

        public MultiSelect(
            FormEvents formEvents,
            string title,
            Func<IEnumerable<PossibilityItem>> getPossibilities,
            MultiSelectOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            if (getPossibilities is null)
                throw new ArgumentNullException(nameof(getPossibilities));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? new MultiSelectOptions();
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

        public MultiSelect(
            FormEvents formEvents,
            string title,
            IEnumerable<string> possibilities,
            MultiSelectOptions options)
            : this(
                  formEvents,
                  title,
                  () => possibilities?.Select(e => new PossibilityItem(e))!,
                  options)
        {
        }

        internal Writer Writer { get; }
        internal KeyHandler KeyHandler { get; }
        internal MultiSelectErrorEnum? Error { get; set; }
        internal Action<IEnumerable<PossibilityItem>> OnConfirmAction = (e) => { };
        internal Action<PossibilityItem> OnChangeAction = (e) => { };
        internal Action OnResetAction = () => { };

        public string Title { get; private set; }
        public PossibilityItem[] Possibilities { get; private set; } = null!;
        public MultiSelectOptions Options { get; }

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

        private MultiSelectAnswer RunInternal()
        {
            ConsoleWriter.SetCursorVisibility(false);

            var currentIndex = 0;

            Writer.Print();

            var position = ConsoleWriter.GetCursorPosition();
            var errorLine = position.Top;

            MultiSelectAnswer? answer = null;

            while (answer is null)
            {
                Writer.PrintError(ref errorLine);

                var key = ConsoleWriter.ReadKey(true);

                KeyHandler.HandleKey(ref currentIndex, key);
                answer = KeyHandler.GetAnswer(key);
            }

            return answer;
        }

        public MultiSelect OnConfirm(Action<IEnumerable<PossibilityItem>> onConfirm)
        {
            OnConfirmAction += onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));
            return this;
        }

        public MultiSelect OnChange(Action<PossibilityItem> onChange)
        {
            OnChangeAction += onChange ?? throw new ArgumentNullException(nameof(onChange));
            return this;
        }

        public MultiSelect OnReset(Action onReset)
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
