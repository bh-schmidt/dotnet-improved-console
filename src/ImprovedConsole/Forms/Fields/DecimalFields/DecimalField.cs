using ImprovedConsole.Extensions;

namespace ImprovedConsole.Forms.Fields.DecimalFields
{
    public class DecimalField : IField, IResettable
    {
        private readonly FormEvents formEvents;
        private Action<decimal?> OnConfirmEvent = (e) => { };
        private Action<decimal?> OnResetAction = (e) => { };
        private Func<decimal?, decimal?> ProcessDataBeforeValidations = (e) => e;
        private Func<decimal?, decimal?> ProcessDataAfterValidations = (e) => e;
        private Func<decimal?, string?> GetValidation = (e) => null;
        private Func<decimal?, IEnumerable<string>> GetValidations;
        private InitialValue<decimal?>? initialValue;

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

            GetValidations = (e) =>
            {
                string? res = GetValidation(e);
                if (res != null)
                    return [res];

                return [];
            };
        }

        public string Title { get; private set; }
        public DecimalFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            decimal? value = null;
            IEnumerable<string> validationErrors = [];

            if (ReturnInitialValue())
            {
                var answer = new DecimalFieldAnswer(this, initialValue!.Value);
                initialValue = null;
                return answer;
            }

            while (true)
            {
                formEvents.Reprint();
                string? readValue = Read(validationErrors);

                decimal? convertedValue = decimal.TryParse(readValue, out decimal parsed) ?
                    parsed :
                    null;

                if (Options.Required && convertedValue is null)
                    continue;

                if (!string.IsNullOrWhiteSpace(readValue) && convertedValue is null)
                    continue;

                decimal? preValidationValue = ProcessDataBeforeValidations(convertedValue);

                validationErrors = GetValidations(preValidationValue);
                if (!validationErrors.IsNullOrEmpty())
                    continue;

                value = ProcessDataAfterValidations(preValidationValue);
                break;
            }

            OnConfirmEvent(value);
            return new DecimalFieldAnswer(this, value);
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

        public DecimalField WithValue(decimal? initialValue)
        {
            this.initialValue = new InitialValue<decimal?>(initialValue);
            return this;
        }

        public DecimalField OnConfirm(Action<decimal?> onConfirm)
        {
            ArgumentNullException.ThrowIfNull(onConfirm);

            OnConfirmEvent += onConfirm;
            return this;
        }

        public DecimalField OnReset(Action<decimal?> onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        public DecimalField SetValidation(Func<decimal?, string?> getValidation)
        {
            GetValidation += getValidation;
            return this;
        }

        public DecimalField SetValidations(Func<decimal?, IEnumerable<string>> getValidations)
        {
            GetValidations += getValidations;
            return this;
        }

        public DecimalField SetDataProcessingBeforeValidations(Func<decimal?, decimal?> processData)
        {
            ProcessDataBeforeValidations += processData;
            return this;
        }

        public DecimalField SetDataProcessingAfterValidations(Func<decimal?, decimal?> processData)
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

            if (answer is DecimalFieldAnswer a)
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
