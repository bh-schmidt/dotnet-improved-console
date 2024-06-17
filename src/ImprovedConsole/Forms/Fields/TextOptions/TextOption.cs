using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOption<TFieldType>(
        FormEvents formEvents) :
        Field<TFieldType, TextOption<TFieldType>>,
        IField,
        IResettable,
        IOptions<TFieldType, TextOption<TFieldType>>,
        ISetValue<TFieldType?, TextOption<TFieldType>>,
        IDefaultValue<TFieldType?, TextOption<TFieldType>>,
        IOnConfirm<TFieldType?, TextOption<TFieldType>>,
        IOnReset<TFieldType?, TextOption<TFieldType>>
    {
        private readonly FormEvents formEvents = formEvents;

        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; set; }
        public Func<TFieldType?>? GetValueToSetEvent { get; set; }
        public Action<TFieldType?>? OnConfirmEvent { get; set; }
        public Action<TFieldType?>? OnResetEvent { get; set; }
        public Func<TFieldType?>? GetDefaultValueEvent { get; set; }

        public override IFieldAnswer Run()
        {
            ValidateField();
            IEnumerable<string>? validationErrors = [];

            var title = getTitle();
            var required = isRequired();
            var options = GetOptionsEvent!();

            var valuesToSet = GetValueToSetEvent is null ?
                default :
                GetValueToSetEvent();

            var defaultValue = GetDefaultValueEvent is null ?
                default :
                GetDefaultValueEvent();

            if (GetDefaultValueEvent is not null)
                OptionNotAllowedException.ThrowIfInvalid(defaultValue, options);

            if (ShouldSetValue(required, valuesToSet, options))
            {
                var answer = new TextOptionAnswer<TFieldType>(this, title, valuesToSet);
                GetValueToSetEvent = default;
                return answer;
            }

            TFieldType? value;

            while (true)
            {
                formEvents.Reprint();
                var readValue = Read(title, options, validationErrors);

                if (string.IsNullOrEmpty(readValue))
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

                    validationErrors = ["This field is required."];
                    continue;
                }

                if (!TryConvert(readValue, out value))
                {
                    validationErrors = ["Could not convert the value."];
                    continue;
                }

                if (!options.Contains(value))
                {
                    validationErrors = ["Invalid option."];
                    continue;
                }

                break;
            }

            OnConfirmEvent?.Invoke(value);

            return new TextOptionAnswer<TFieldType>(this, title, value);
        }

        private string? Read(string title, IEnumerable<TFieldType> options, IEnumerable<string> validationErrors)
        {
            ConsoleColor color = ConsoleWriter.GetForegroundColor();

            Message.Write(title);

            IEnumerable<string?> stringOptions = options.Select(e => e?.ToString());
            string optionsText = $"({string.Join("/", stringOptions)})";
            ConsoleWriter
                .Write(' ')
                .Write(optionsText)
                .WriteLine();

            if (!validationErrors.IsNullOrEmpty())
            {
                foreach (string item in validationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }

            return ConsoleWriter.ReadLine();
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            if (answer is TextOptionAnswer<TFieldType> a)
            {
                OnResetEvent?.Invoke(a.Answer);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }

        private bool ShouldSetValue(bool required, TFieldType? initialValue, IEnumerable<TFieldType> options)
        {
            if (GetValueToSetEvent is null)
                return false;

            if (required && initialValue is null)
                return false;

            return initialValue is null || options.Contains(initialValue);
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
            GetValueToSetEvent += getValue;
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
