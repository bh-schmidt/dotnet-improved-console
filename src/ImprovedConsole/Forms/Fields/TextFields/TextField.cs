using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Fields.DecimalFields;
using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextField : IField, IResettable
    {
        private readonly FormEvents formEvents;
        private Action<string?> OnConfirmEvent = (e) => { };
        private Action<string?> OnResetAction = (e) => { };
        private Func<string?, string?> ProcessDataBeforeValidations = (e) => e;
        private Func<string?, string?> ProcessDataAfterValidations = (e) => e;
        private Func<string?, string?> GetValidation = (e) => null;
        private Func<string?, IEnumerable<string>> GetValidations;
        private InitialValue<string?>? initialValue;

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
                string? res = GetValidation(e);
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

            if (ReturnInitialValue())
            {
                var answer = new TextFieldAnswer(this, initialValue!.Value);
                initialValue = null;
                return answer;
            }

            while (true)
            {
                formEvents.Reprint();
                string? readValue = Read(validationErrors);

                if (Options.Required && string.IsNullOrEmpty(readValue))
                    continue;

                string? preValidationValue = ProcessDataBeforeValidations(readValue);

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
                foreach (string item in validationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }

            line = ConsoleWriter.ReadLine();
            return line;
        }

        public TextField WithValue(string? initialValue)
        {
            this.initialValue = new InitialValue<string?>(initialValue);
            return this;
        }

        public TextField OnConfirm(Action<string?> onConfirm)
        {
            ArgumentNullException.ThrowIfNull(onConfirm);

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

        void IResettable.Reset(IFieldAnswer? answer)
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

        private bool ReturnInitialValue()
        {
            if (initialValue is null)
                return false;

            if (initialValue.Value is not null)
                return true;

            return !Options.Required;
        }
    }
}
