using ImprovedConsole.Extensions;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextField : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Action<string?> OnConfirmEvent = (e) => { };
        private Action<string?> OnResetAction = (e) => { };
        private Func<string?, string?> ProcessDataBeforeValidations = (e) => e;
        private Func<string?, string?> ProcessDataAfterValidations = (e) => e;
        private Func<string?, string?> GetValidation = (e) => null;
        private Func<string?, IEnumerable<string>> GetValidations;

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

            GetValidations = (e) =>
            {
                var res = GetValidation(e);
                if (res != null)
                    return [res];

                return [];
            };
        }

        public string Title { get; private set; }
        public TextFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            string? value = null;
            IEnumerable<string>? validationErrors = [];

            while (true)
            {
                formEvents.Reprint();
                var readValue = Read(validationErrors);

                if (Options.Required && string.IsNullOrEmpty(readValue))
                    continue;

                var preValidationValue = ProcessDataBeforeValidations(readValue);

                validationErrors = GetValidations(preValidationValue);
                if (!validationErrors.IsNullOrEmpty())
                    continue;

                value = ProcessDataAfterValidations(preValidationValue);
                break;
            }

            if (string.IsNullOrEmpty(value))
                value = null;

            OnConfirmEvent(value);
            return new TextFieldAnswer(this, value);
        }

        private string? Read(IEnumerable<string> validationErrors)
        {
            string? line;
            Message.WriteLine(Title);

            if (!validationErrors.IsNullOrEmpty())
            {
                foreach (var item in validationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }

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

        public TextField OnReset(Action<string?> onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        public TextField SetValidation(Func<string?, string?> getValidation)
        {
            GetValidation += getValidation;
            return this;
        }

        public TextField SetValidations(Func<string?, IEnumerable<string>> getValidations)
        {
            GetValidations += getValidations;
            return this;
        }

        public TextField SetDataProcessingBeforeValidations(Func<string?, string?> processData)
        {
            ProcessDataBeforeValidations += processData;
            return this;
        }

        public TextField SetDataProcessingAfterValidations(Func<string?, string?> processData)
        {
            ProcessDataAfterValidations += processData;
            return this;
        }

        void IResetable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetAction(null);
                return;
            }

            if (answer is TextFieldAnswer a)
            {
                OnResetAction(a.Answer);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }
    }
}
