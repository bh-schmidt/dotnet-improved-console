using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextField<TFieldType>(
        FormEvents formEvents) :
        Field<TFieldType, TextField<TFieldType>>,
        IField,
        ISetValue<TFieldType?, TextField<TFieldType>>,
        IDefaultValue<TFieldType?, TextField<TFieldType>>,
        IOnConfirm<TFieldType?, TextField<TFieldType>>,
        IOnReset<TFieldType?, TextField<TFieldType>>,
        ISetValidation<TextField<TFieldType>>,
        ITransformOnRead<TextField<TFieldType>>,
        IITransformOnValidate<TextField<TFieldType>>
    {
        internal FormEvents FormEvents { get; } = formEvents;

        public Func<TFieldType?>? GetExternalValueEvent { get; private set; }
        public Action<TFieldType?>? OnConfirmEvent { get; private set; }
        public Action<TFieldType?>? OnResetEvent { get; private set; }
        public Func<TFieldType?>? GetDefaultValueEvent { get; private set; }
        public Func<string, string?>? ValidateEvent { get; private set; }
        public Func<string, string>? TransformOnReadEvent { get; private set; }
        public Func<string, string>? TransformOnValidateEvent { get; private set; }

        public override IFieldAnswer Run()
        {
            try
            {
                ValidateField();

                var answer = RunInternal();
                Answer = answer;
                return answer;
            }
            finally
            {
                Finished = true;
                IsEditing = false;
            }
        }

        public IFieldAnswer RunInternal()
        {
            var title = GetTitle();
            var required = IsRequired();

            var externalValue = GetExternalValue(title, required);
            if (externalValue is not null)
                return externalValue;

            var defaultValue = GetDefaultValue(required);

            var runner = new TextFieldRunner<TFieldType>(
                this,
                title,
                required,
                defaultValue);

            return runner.Run();
        }

        private TFieldType? GetDefaultValue(bool required)
        {
            if (GetDefaultValueEvent is null)
                return default;

            if (required)
                return default;

            return GetDefaultValueEvent();
        }

        private TextFieldAnswer<TFieldType>? GetExternalValue(string title, bool required)
        {
            if (GetExternalValueEvent is null)
                return null;

            if (IsEditing)
                return null;

            var externalValue = GetExternalValueEvent();
            if (!required || externalValue is not null)
            {
                var answer = new TextFieldAnswer<TFieldType>(this, title, externalValue);
                OnConfirmEvent?.Invoke(externalValue);
                return answer;
            }

            return null;
        }

        public override TextField<TFieldType> ValidateField()
        {
            base.ValidateField();
            ConverterNotSetException.ThrowIfNull(ConvertFromStringDelegate);
            return this;
        }

        public override void Reset()
        {
            var answer = Answer as TextFieldAnswer<TFieldType>;

            Answer = null;
            Finished = false;

            if (answer is null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            OnResetEvent?.Invoke(answer!.Answer);
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
            GetExternalValueEvent += getValue;
            return this;
        }

        public TextField<TFieldType> Set(TFieldType? value)
        {
            return Set(() => value);
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
