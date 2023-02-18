﻿namespace ImprovedConsole.Forms.Fields.OptionSelectors
{
    public class OptionSelector : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Func<HashSet<string>> getPossibilities { get; }
        private Action<string?> OnConfirmAction = (e) => { };
        private Action OnResetAction = () => { };

        public OptionSelector(
            FormEvents formEvents,
            string title,
            Func<IEnumerable<string>> getPossibilities,
            OptionSelectorsOptions options)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException($"'{nameof(title)}' cannot be null or empty.", nameof(title));

            if (getPossibilities is null)
                throw new ArgumentNullException(nameof(getPossibilities));

            this.formEvents = formEvents;
            Title = title;
            Options = options ?? throw new ArgumentNullException(nameof(options));


            this.getPossibilities = () =>
            {
                var possibilities = getPossibilities();
                if (!possibilities.Any())
                    throw new ArgumentException($"'{nameof(possibilities)}' cannot be null or empty.", nameof(title));

                return possibilities.ToHashSet();
            };
        }

        public OptionSelector(
            FormEvents formEvents,
            string title,
            IEnumerable<string> possibilities,
            OptionSelectorsOptions options)
            : this(
                  formEvents,
                  title,
                  () => possibilities,
                  options)
        {
        }

        public string Title { get; private set; }
        public HashSet<string> Possibilities { get; private set; } = null!;
        public OptionSelectorsOptions Options { get; }

        public IFieldAnswer Run()
        {
            Possibilities = getPossibilities();
            string? value = Read();

            while (ShouldRepeatLoop(value))
            {
                formEvents.Reprint();
                value = Read();
            }

            if (string.IsNullOrEmpty(value))
                value = null;

            OnConfirmAction(value);

            return new OptionSelectorsAnswer(this, value);
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
            var color = ConsoleWriter.GetForegroundColor();

            Message.Write(Title);

            if (Options.ShowOptions)
            {
                var optionsText = $"({string.Join("/", Possibilities)})";
                ConsoleWriter.Write(' ');
                ConsoleWriter.SetForegroundColor(Options.OptionsColor);
                ConsoleWriter.Write(optionsText);
                ConsoleWriter.SetForegroundColor(color);
            }

            ConsoleWriter.WriteLine();

            return ConsoleWriter.ReadLine();
        }

        public OptionSelector OnConfirm(Action<string?> onConfirm)
        {
            OnConfirmAction += onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));
            return this;
        }

        public OptionSelector OnReset(Action onReset)
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
