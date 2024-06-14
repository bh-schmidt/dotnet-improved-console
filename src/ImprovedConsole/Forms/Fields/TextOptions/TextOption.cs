using ImprovedConsole.Forms.Fields.DecimalFields;

namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOption : IField, IResettable
    {
        private readonly FormEvents formEvents;
        private readonly Func<HashSet<string>> getPossibilities;
        private Action<string?> OnConfirmAction = (e) => { };
        private Action<string?> OnResetAction = (e) => { };
        private InitialValue<string?>? initialValue;

        public TextOption(
            FormEvents formEvents,
            string title,
            Func<IEnumerable<string>> getPossibilities,
            TextOptionOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            ArgumentNullException.ThrowIfNull(getPossibilities);

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? throw new ArgumentNullException(nameof(options));


            this.getPossibilities = () =>
            {
                IEnumerable<string> possibilities = getPossibilities();
                if (!possibilities.Any())
                    throw new ArgumentException($"'{nameof(possibilities)}' cannot be null or empty.", nameof(title));

                return possibilities.ToHashSet();
            };
        }

        public TextOption(
            FormEvents formEvents,
            string title,
            IEnumerable<string> possibilities,
            TextOptionOptions options)
            : this(
                  formEvents,
                  title,
                  () => possibilities,
                  options)
        {
        }

        public string Title { get; private set; }
        public HashSet<string> Possibilities { get; private set; } = null!;
        public TextOptionOptions Options { get; }

        public IFieldAnswer Run()
        {
            Possibilities = getPossibilities();

            if (ReturnInitialValue())
            {
                var answer = new TextOptionAnswer(this, initialValue!.Value);
                initialValue = null;
                return answer;
            }

            string? value = Read();

            while (ShouldRepeatLoop(value))
            {
                formEvents.Reprint();
                value = Read();
            }

            if (string.IsNullOrEmpty(value))
                value = null;

            OnConfirmAction(value);

            return new TextOptionAnswer(this, value);
        }

        private bool ShouldRepeatLoop(string? value)
        {
            if (Options.Required && string.IsNullOrEmpty(value))
                return true;

            if (string.IsNullOrEmpty(value))
                return false;

            return !Possibilities.Contains(value);
        }

        private string? Read()
        {
            ConsoleColor color = ConsoleWriter.GetForegroundColor();

            Message.Write(Title);

            if (Options.ShowOptions)
            {
                string optionsText = $"({string.Join("/", Possibilities)})";
                ConsoleWriter.Write(' ');
                ConsoleWriter.SetForegroundColor(Options.OptionsColor);
                ConsoleWriter.Write(optionsText);
                ConsoleWriter.SetForegroundColor(color);
            }

            ConsoleWriter.WriteLine();

            return ConsoleWriter.ReadLine();
        }

        public TextOption WithValue(string? initialValue)
        {
            this.initialValue = new InitialValue<string?>(initialValue);
            return this;
        }

        public TextOption OnConfirm(Action<string?> onConfirm)
        {
            OnConfirmAction += onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));
            return this;
        }

        public TextOption OnReset(Action<string?> onReset)
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

            if (answer is TextOptionAnswer a)
            {
                OnResetAction(a.Answer);
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

            return initialValue.Value is null || Possibilities.Contains(initialValue.Value);
        }
    }
}
