using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Fields.DecimalFields;

namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongField : IField, IResettable
    {
        private readonly FormEvents formEvents;
        private Action<long?> OnConfirmEvent = (e) => { };
        private Action<long?> OnResetAction = (e) => { };
        private Func<long?, long?> ProcessDataBeforeValidations = (e) => e;
        private Func<long?, long?> ProcessDataAfterValidations = (e) => e;
        private Func<long?, string?> GetValidation = (e) => null;
        private Func<long?, IEnumerable<string>> GetValidations;
        private InitialValue<long?>? initialValue;

        public LongField(
            FormEvents formEvents,
            string title,
            LongFieldOptions options)
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
        public LongFieldOptions Options { get; }

        public IFieldAnswer Run()
        {
            long? value = null;
            IEnumerable<string> validationErrors = [];

            if (ReturnInitialValue())
            {
                var answer = new LongFieldAnswer(this, initialValue!.Value);
                initialValue = null;
                return answer;
            }

            while (true)
            {
                formEvents.Reprint();
                string? readValue = Read(validationErrors);

                long? convertedValue = long.TryParse(readValue, out long parsed) ?
                    parsed :
                    null;

                if (Options.Required && convertedValue is null)
                    continue;

                if (!string.IsNullOrWhiteSpace(readValue) && convertedValue is null)
                    continue;

                long? preValidationValue = ProcessDataBeforeValidations(convertedValue);

                validationErrors = GetValidations(preValidationValue);
                if (!validationErrors.IsNullOrEmpty())
                    continue;

                value = ProcessDataAfterValidations(preValidationValue);
                break;
            }

            OnConfirmEvent(value);
            return new LongFieldAnswer(this, value);
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

        public LongField WithValue(long? initialValue)
        {
            this.initialValue = new InitialValue<long?>(initialValue);
            return this;
        }

        public LongField OnConfirm(Action<long?> onConfirm)
        {
            ArgumentNullException.ThrowIfNull(onConfirm);

            OnConfirmEvent += onConfirm;
            return this;
        }

        public LongField OnReset(Action<long?> onReset)
        {
            OnResetAction += onReset ?? throw new ArgumentNullException(nameof(onReset));
            return this;
        }

        public LongField SetValidation(Func<long?, string?> getValidation)
        {
            GetValidation += getValidation;
            return this;
        }

        public LongField SetValidations(Func<long?, IEnumerable<string>> getValidations)
        {
            GetValidations += getValidations;
            return this;
        }

        public LongField SetDataProcessingBeforeValidations(Func<long?, long?> processData)
        {
            ProcessDataBeforeValidations += processData;
            return this;
        }

        public LongField SetDataProcessingAfterValidations(Func<long?, long?> processData)
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

            if (answer is LongFieldAnswer a)
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
