using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;
using System.Linq;

namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelect<TFieldType>(FormEvents formEvents) :
        Field<TFieldType, SingleSelect<TFieldType>>,
        IField,
        IOptions<TFieldType, SingleSelect<TFieldType>>,
        IDefaultValue<TFieldType?, SingleSelect<TFieldType>>,
        ISelectedValue<TFieldType?, SingleSelect<TFieldType>>,
        ISetValue<TFieldType?, SingleSelect<TFieldType>>,
        IOnChange<TFieldType?, SingleSelect<TFieldType>>,
        IOnConfirm<TFieldType?, SingleSelect<TFieldType>>,
        IOnReset<TFieldType?, SingleSelect<TFieldType>>
    {
        internal FormEvents FormEvents { get; } = formEvents;

        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; private set; }
        public Func<TFieldType?>? GetExternalValueEvent { get; private set; }
        public Action<TFieldType?>? OnConfirmEvent { get; private set; }
        public Action<TFieldType?>? OnResetEvent { get; private set; }
        public Action<TFieldType?>? OnChangeEvent { get; private set; }
        public Func<TFieldType?>? GetSelectedValueEvent { get; private set; }
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

        private SingleSelectAnswer<TFieldType> RunInternal()
        {
            var title = GetTitle();
            var required = IsRequired();
            var options = GetOptionsEvent!().ToList();
            var optionItems = options.Select(e => new OptionItem<TFieldType>(e)).ToList();

            var externalValue = GetExternalValue(title, required, options);
            if (externalValue is not null)
                return externalValue;

            if (required && options.Count == 1 )
            {
                var value = options[0];
                var answer = new SingleSelectAnswer<TFieldType>(this, title, value);
                OnConfirmEvent?.Invoke(value);
                return answer;
            }

            var defaultItem = GetDefaultItem(options, required);
            var selection = GetSelection(options, optionItems, required);

            var runner = new SingleSelectRunner<TFieldType>(
                this,
                title,
                required,
                optionItems,
                defaultItem,
                selection);

            return runner.Run();
        }

        private OptionItem<TFieldType>? GetSelection(List<TFieldType> options, List<OptionItem<TFieldType>> optionItems, bool required)
        {
            if (Answer is not null)
            {
                var answer = (SingleSelectAnswer<TFieldType>)Answer;

                if (answer.Selection is not null && options.Contains(answer.Selection))
                {
                    var index = options.IndexOf(answer.Selection);
                    var selection = optionItems[index];
                    selection.Checked = true;
                    return selection;
                }
            }

            if (GetSelectedValueEvent is not null)
            {
                var selectedOption = GetSelectedValueEvent.Invoke();

                if (selectedOption is not null)
                {
                    OptionNotAllowedException.ThrowIfInvalid(selectedOption, options);
                    var index = options.IndexOf(selectedOption);
                    var selection = optionItems[index];
                    selection.Checked = true;
                    return selection;
                }
            }

            if (required)
            {
                var selection = optionItems.First();
                selection.Checked = true;
                return selection;
            }

            return null;
        }

        private SingleSelectAnswer<TFieldType>? GetExternalValue(string title, bool required, List<TFieldType> options)
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
                var answer = new SingleSelectAnswer<TFieldType>(this, title, externalValue);
                OnConfirmEvent?.Invoke(externalValue);
                return answer;
            }

            return null;
        }

        private OptionItem<TFieldType>? GetDefaultItem(List<TFieldType> options, bool required)
        {
            if (GetDefaultValueEvent is null)
                return null;

            if (required)
                return null;

            var defaultValue = GetDefaultValueEvent();
            if (defaultValue is null)
                return null;

            var defaultItem = new OptionItem<TFieldType>(defaultValue!);
            OptionNotAllowedException.ThrowIfInvalid(defaultValue, options);

            return defaultItem;
        }

        public SingleSelect<TFieldType> OnChange(Action<TFieldType?> callback)
        {
            OnChangeEvent += callback ?? throw new ArgumentNullException(nameof(callback));
            return this;
        }

        public override void Reset()
        {
            var answer = Answer as SingleSelectAnswer<TFieldType>;

            Answer = null;
            Finished = false;

            if (answer is null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            if (answer!.Selection is null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            OnResetEvent?.Invoke(answer.Selection);
        }

        public SingleSelect<TFieldType> Options(Func<IEnumerable<TFieldType>> getOptions)
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

        public SingleSelect<TFieldType> Options(IEnumerable<TFieldType> options)
        {
            EmptyOptionsException.ThrowIfNullOrEmpty(options);
            DuplicateOptionException.ThrowIfDuplicated(options);
            return Options(() => options);
        }

        public SingleSelect<TFieldType> Set(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetExternalValueEvent += getValue;
            return this;
        }

        public SingleSelect<TFieldType> Set(TFieldType? value)
        {
            return Set(() => value);
        }

        public SingleSelect<TFieldType> OnConfirm(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnConfirmEvent += callback;
            return this;
        }

        public SingleSelect<TFieldType> OnReset(Action<TFieldType?> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnResetEvent += callback;
            return this;
        }

        public override SingleSelect<TFieldType> ValidateField()
        {
            base.ValidateField();
            OptionsNotSetException.ThrowIfNull(GetOptionsEvent);
            return this;
        }

        public SingleSelect<TFieldType> Selected(Func<TFieldType?> getValues)
        {
            ArgumentNullException.ThrowIfNull(getValues, nameof(getValues));
            GetSelectedValueEvent += getValues;
            return this;
        }

        public SingleSelect<TFieldType> Selected(TFieldType? value)
        {
            return Selected(() => value);
        }

        public SingleSelect<TFieldType> Default(Func<TFieldType?> getValue)
        {
            ArgumentNullException.ThrowIfNull(getValue, nameof(getValue));
            GetDefaultValueEvent += getValue;
            return this;
        }

        public SingleSelect<TFieldType> Default(TFieldType? value)
        {
            return Default(() => value);
        }
    }
}
