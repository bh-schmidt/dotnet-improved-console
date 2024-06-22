using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOption<TFieldType>(
        FormEvents formEvents) :
        Field<TFieldType, TextOption<TFieldType>>,
        IField,
        IOptions<TFieldType, TextOption<TFieldType>>,
        ISetValue<TFieldType?, TextOption<TFieldType>>,
        IDefaultValue<TFieldType?, TextOption<TFieldType>>,
        IOnConfirm<TFieldType?, TextOption<TFieldType>>,
        IOnReset<TFieldType?, TextOption<TFieldType>>
    {
        internal FormEvents FormEvents { get; } = formEvents;

        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; private set; }
        public Func<TFieldType?>? GetExternalValueEvent { get; private set; }
        public Action<TFieldType?>? OnConfirmEvent { get; private set; }
        public Action<TFieldType?>? OnResetEvent { get; private set; }
        public Func<TFieldType?>? GetDefaultValueEvent { get; private set; }

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
            IEnumerable<string>? validationErrors = [];

            var title = GetTitle();
            var required = IsRequired();
            var options = GetOptionsEvent!();

            var externalValue = GetExternalValue(title, required, options);
            if (externalValue is not null)
                return externalValue;

            var defaultValue = GetDefaultValue(options, required);

            var runner = new TextOptionRunner<TFieldType>(
                this,
                title,
                required,
                options,
                defaultValue);
            return runner.Run();
        }

        private TFieldType? GetDefaultValue(IEnumerable<TFieldType> options, bool required)
        {
            if (GetDefaultValueEvent is null)
                return default;

            if (required)
                return default;

            var defaultValue = GetDefaultValueEvent();
            OptionNotAllowedException.ThrowIfInvalid(defaultValue, options);

            return defaultValue;
        }

        private TextOptionAnswer<TFieldType>? GetExternalValue(string title, bool required, IEnumerable<TFieldType> options)
        {
            if (GetExternalValueEvent is null)
                return null;

            if (IsEditing)
                return null;

            var externalValue = GetExternalValueEvent();
            if (required && externalValue is null)
                return null;

            if (externalValue is null || options.Contains(externalValue))
            {
                var answer = new TextOptionAnswer<TFieldType>(this, title, externalValue);
                OnConfirmEvent?.Invoke(externalValue);
                return answer;
            }

            return null;
        }

        public override void Reset()
        {
            var answer = Answer as TextOptionAnswer<TFieldType>;

            Answer = null;
            Finished = false;

            if (answer is null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            OnResetEvent?.Invoke(answer!.Answer);
        }

        public TextOption<TFieldType> Options(Func<IEnumerable<TFieldType>> getOptions)
        {
            ArgumentNullException.ThrowIfNull(getOptions, nameof(getOptions));
            GetOptionsEvent += () =>
            {
                var options = getOptions();
                EmptyOptionsException.ThrowIfNullOrEmpty(options);
                DuplicateOptionException.ThrowIfDuplicated(options);
                return options;
            };
            return this;
        }

        public TextOption<TFieldType> Options(IEnumerable<TFieldType> options)
        {
            EmptyOptionsException.ThrowIfNullOrEmpty(options);
            DuplicateOptionException.ThrowIfDuplicated(options);
            return Options(() => options);
        }

        public TextOption<TFieldType> Set(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetExternalValueEvent += getValue;
            return this;
        }

        public TextOption<TFieldType> Set(TFieldType? value)
        {
            return Set(() => value);
        }

        public TextOption<TFieldType> OnConfirm(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnConfirmEvent += callback;
            return this;
        }

        public TextOption<TFieldType> OnReset(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnResetEvent += callback;
            return this;
        }

        public override TextOption<TFieldType> ValidateField()
        {
            base.ValidateField();
            OptionsNotSetException.ThrowIfNull(GetOptionsEvent);
            ConverterNotSetException.ThrowIfNull(ConvertFromStringDelegate);
            return this;
        }

        public TextOption<TFieldType> Default(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetDefaultValueEvent += getValue;
            return this;
        }

        public TextOption<TFieldType> Default(TFieldType? value)
        {
            return Default(() => value);
        }
    }
}
