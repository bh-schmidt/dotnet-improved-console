using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Fields.DecimalFields;

namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongField : IField, IResetable
    {
        private readonly FormEvents formEvents;
        private Action<long?> OnConfirmEvent = (e) => { };
        private Action<long?> OnResetAction = (e) => { };
        private Func<long?, long?> ProcessDataBeforeValidations = (e) => e;
        private Func<long?, long?> ProcessDataAfterValidations = (e) => e;
        private Func<long?, string?> GetValidation = (e) => null;
        private Func<long?, IEnumerable<string>> GetValidations;

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
                var res = GetValidation(e);
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

            while (true)
            {
                formEvents.Reprint();
                var readValue = Read(validationErrors);

                long? convertedValue = long.TryParse(readValue, out long parsed) ?
                    parsed :
                    null;

                if (Options.Required && convertedValue is null)
                    continue;

                if (!string.IsNullOrWhiteSpace(readValue) && convertedValue is null)
                    continue;

                var preValidationValue = ProcessDataBeforeValidations(convertedValue);

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
                foreach (var item in validationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }

            line = ConsoleWriter.ReadLine();
            return line;
        }

        public LongField OnConfirm(Action<long?> onConfirm)
        {
            if (onConfirm is null)
                throw new ArgumentNullException(nameof(onConfirm));

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

        void IResetable.Reset(IFieldAnswer? answer)
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
    }
}
