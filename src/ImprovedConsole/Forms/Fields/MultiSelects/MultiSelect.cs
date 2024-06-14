using ImprovedConsole.Forms.Fields.DecimalFields;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelect : IField, IResettable
    {
        private readonly FormEvents formEvents;
        private readonly Func<PossibilityItem[]> getPossibilities;
        internal Action<IEnumerable<PossibilityItem>> OnConfirmAction = (e) => { };
        internal Action<PossibilityItem> OnChangeAction = (e) => { };
        internal Action<IEnumerable<PossibilityItem>> OnResetAction = (e) => { };
        private InitialValue<IEnumerable<string>>? initialValue;

        public MultiSelect(
            FormEvents formEvents,
            string title,
            Func<IEnumerable<PossibilityItem>> getPossibilities,
            MultiSelectOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            ArgumentNullException.ThrowIfNull(getPossibilities);

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? new MultiSelectOptions();
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
        public string Title { get; private set; }
        public PossibilityItem[] Possibilities { get; private set; } = null!;
        public MultiSelectOptions Options { get; }

        public IFieldAnswer Run()
        {
            Possibilities = getPossibilities();

            var possibilitiesHash = Possibilities.ToDictionary(e => e.Value);
            if (ReturnInitialValue(possibilitiesHash))
            {
                var answer = new MultiSelectAnswer(this, initialValue!.Value?.Select(e => possibilitiesHash[e]) ?? []);
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

        private MultiSelectAnswer RunInternal()
        {
            ConsoleWriter.SetCursorVisibility(false);

            int currentIndex = 0;

            Writer.Print();
            (_, int Top) = ConsoleWriter.GetCursorPosition();
            int errorLine = Top;

            MultiSelectAnswer? answer = null;

            while (answer is null)
            {
                Writer.PrintError(ref errorLine);

                ConsoleKeyInfo key = ConsoleWriter.ReadKey(true);

                KeyHandler.HandleKey(ref currentIndex, key);
                answer = KeyHandler.GetAnswer(key);
            }

            return answer;
        }

        public MultiSelect WithValues(IEnumerable<string> initialValue)
        {
            this.initialValue = new InitialValue<IEnumerable<string>>(initialValue);
            return this;
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

        public MultiSelect OnReset(Action<IEnumerable<PossibilityItem>> onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetAction([]);
                return;
            }

            if (answer is MultiSelectAnswer a)
            {
                OnResetAction(a.SelectedItems);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }

        private bool ReturnInitialValue(Dictionary<string, PossibilityItem> possibilitiesHash)
        {
            if (initialValue is null)
                return false;

            if (Options.Required && initialValue.Value is null)
                return false;

            return initialValue.Value is null || initialValue.Value.All(possibilitiesHash.ContainsKey);
        }
    }
}
