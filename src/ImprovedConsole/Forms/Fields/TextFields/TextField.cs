using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextField<TFieldType>(
        FormEvents formEvents) :
        Field<TFieldType, TextField<TFieldType>>,
        IField,
        IResettable,
        ISetValue<TFieldType?, TextField<TFieldType>>,
        IDefaultValue<TFieldType?, TextField<TFieldType>>,
        IOnConfirm<TFieldType?, TextField<TFieldType>>,
        IOnReset<TFieldType?, TextField<TFieldType>>,
        ISetValidation<TextField<TFieldType>>,
        ITransformOnRead<TextField<TFieldType>>,
        IITransformOnValidate<TextField<TFieldType>>
    {
        private readonly FormEvents formEvents = formEvents;

        public Func<TFieldType?>? GetValueToSetEvent { get; set; }
        public Action<TFieldType?>? OnConfirmEvent { get; set; }
        public Action<TFieldType?>? OnResetEvent { get; set; }
        public Func<TFieldType?>? GetDefaultValueEvent { get; set; }
        public Func<string, string?>? ValidateEvent { get; set; }
        public Func<string, string>? TransformOnReadEvent { get; set; }
        public Func<string, string>? TransformOnValidateEvent { get; set; }

        public override IFieldAnswer Run()
        {
            ValidateField();

            TFieldType? value = default;
            string? error = null;

            var title = getTitle();
            var required = isRequired();

            var valuesToSet = GetValueToSetEvent is null ?
                default :
                GetValueToSetEvent();

            var defaultValue = GetDefaultValueEvent is null ?
                default :
                GetDefaultValueEvent();


            if (ShouldSetValue(required, valuesToSet))
            {
                var answer = new TextFieldAnswer<TFieldType>(this, title, valuesToSet, convertToString);
                GetValueToSetEvent = null;
                return answer;
            }

            while (true)
            {
                formEvents.Reprint();
                string? strValue = TextField<TFieldType>.Read(title, error);

                if (string.IsNullOrEmpty(strValue))
                {
                    if (GetDefaultValueEvent is not null)
                    {
                        value = defaultValue;
                        break;
                    }

                    if (!required)
                    {
                        value = default;
                        break;
                    }

                    error = "This field is required.";
                    continue;
                }

                strValue = TransformOnReadEvent?.Invoke(strValue) ?? strValue;

                error = ValidateEvent?.Invoke(strValue);
                if (!error.IsNullOrEmpty())
                    continue;

                strValue = TransformOnValidateEvent?.Invoke(strValue) ?? strValue;

                if (!TryConvertFromString(strValue, out var conversion))
                {
                    error = "Could not convert the value.";
                    continue;
                }

                value = conversion;
                break;
            }

            OnConfirmEvent?.Invoke(value);
            return new TextFieldAnswer<TFieldType>(this, title, value, convertToString);
        }

        private static string? Read(string title, string? error)
        {
            string? line;
            Message.WriteLine(title);

            if (!error.IsNullOrEmpty())
                Message.WriteLine("{color:red} * " + error);

            line = ConsoleWriter.ReadLine();
            return line;
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            if (answer is TextFieldAnswer<TFieldType> a)
            {
                OnResetEvent?.Invoke(a.Answer);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }

        public TextField<TFieldType> OnReset(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnResetEvent += callback;
            return this;
        }

        public TextField<TFieldType> OnConfirm(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnConfirmEvent += callback;
            return this;
        }

        public TextField<TFieldType> Set(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetValueToSetEvent += getValue;
            return this;
        }

        public TextField<TFieldType> Set(TFieldType? value)
        {
            return Set(() => value);
        }

        private bool ShouldSetValue(bool required, TFieldType? initialValue)
        {
            if (GetValueToSetEvent is null)
                return false;

            if (initialValue is not null)
                return true;

            return !required;
        }

        public TextField<TFieldType> Default(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetDefaultValueEvent += getValue;
            return this;
        }

        public TextField<TFieldType> Default(TFieldType? value)
        {
            return Default(() => value);
        }

        public TextField<TFieldType> Validation(Func<string, string?> validate)
        {
            ArgumentNullException.ThrowIfNull(validate, nameof(validate));
            ValidateEvent += validate;
            return this;
        }

        public TextField<TFieldType> TransformOnRead(Func<string, string> transform)
        {
            ArgumentNullException.ThrowIfNull(transform, nameof(transform));
            TransformOnReadEvent += transform;
            return this;
        }

        public TextField<TFieldType> TransformOnValidate(Func<string, string> transform)
        {
            ArgumentNullException.ThrowIfNull(transform, nameof(transform));
            TransformOnValidateEvent += transform;
            return this;
        }
    }
}
