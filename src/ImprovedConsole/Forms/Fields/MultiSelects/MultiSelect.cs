using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelect<TFieldType>(FormEvents formEvents) :
        Field<TFieldType, MultiSelect<TFieldType>>,
        IField,
        IResettable,
        IOptions<TFieldType, MultiSelect<TFieldType>>,
        ISetValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        ISelectedValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IDefaultValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnChange<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnConfirm<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnReset<IEnumerable<TFieldType>, MultiSelect<TFieldType>>
    {
        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; set; }
        public Func<IEnumerable<TFieldType>>? GetValueToSetEvent { get; set; }
        public Func<IEnumerable<TFieldType>>? GetSelectedValueEvent { get; set; }
        public Action<IEnumerable<TFieldType>>? OnChangeEvent { get; set; }
        public Action<IEnumerable<TFieldType>>? OnConfirmEvent { get; set; }
        public Action<IEnumerable<TFieldType>>? OnResetEvent { get; set; }
        public Func<IEnumerable<TFieldType>>? GetDefaultValueEvent { get; set; }

        public override IFieldAnswer Run()
        {
            bool visibility = ConsoleWriter.CanSetCursorVisibility() && ConsoleWriter.GetCursorVisibility();

            try
            {
                ValidateField();

                if (ConsoleWriter.CanSetCursorPosition())
                    ConsoleWriter.SetCursorVisibility(false);
                return RunInternal();
            }
            finally
            {
                if (ConsoleWriter.CanSetCursorPosition())
                    ConsoleWriter.SetCursorVisibility(visibility);
            }
        }

        private MultiSelectAnswer<TFieldType> RunInternal()
        {
            int currentIndex = 0;
            bool selected = false;
            HashSet<OptionItem<TFieldType>> selections = [];

            var title = getTitle();
            var required = isRequired();

            var valuesToSet = GetValueToSetEvent is null ?
                [] :
                GetValueToSetEvent();

            var options = GetOptionsEvent!()
                .ToList();

            var optionItems = options
                .Select(e => new OptionItem<TFieldType>(e))
                .ToArray();

            var defaultValues = GetDefaultValueEvent?.Invoke() ?? [];

            var defaultItems = GetDefaultValueEvent is null ?
                null :
                defaultValues.Select(e => new OptionItem<TFieldType>(e));

            if (GetDefaultValueEvent is not null)
                OptionNotAllowedException.ThrowIfInvalid(defaultValues, options);

            var possibilitiesHash = options.ToHashSet();
            if (ShouldSetValue(required, valuesToSet, possibilitiesHash))
            {
                var answer = new MultiSelectAnswer<TFieldType>(
                    this,
                    title,
                    valuesToSet);
                valuesToSet = null;
                return answer;
            }

            var selectedOptions = (GetSelectedValueEvent?.Invoke() ?? [])
                .ToHashSet();

            if (GetSelectedValueEvent is not null && !selectedOptions.IsNullOrEmpty())
            {
                OptionNotAllowedException.ThrowIfInvalid(selectedOptions, options);

                selections = optionItems
                    .Where(e => selectedOptions.Contains(e.Value))
                    .ToHashSet();

                foreach (var item in selections)
                    item.Checked = true;

                var index = options.IndexOf(selectedOptions.First());
                currentIndex = index;
            }

            var writer = new Writer<TFieldType>(title, optionItems);
            var keyHandler = new KeyHandler<TFieldType>(optionItems);

            (_, int Top) = ConsoleWriter.GetCursorPosition();

            while (!selected)
            {
                if (!ConsoleWriter.CanSetCursorPosition())
                    formEvents.Reprint();

                writer.Print(currentIndex, Top);

                ConsoleKeyInfo key = ConsoleWriter.ReadKey(true);

                if (key.Key == ConsoleKey.DownArrow)
                {
                    keyHandler.IncrementPosition(ref currentIndex);
                    continue;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    keyHandler.DecrementPosition(ref currentIndex);
                    continue;
                }

                if (key.Key == ConsoleKey.Spacebar)
                {
                    var option = optionItems[currentIndex];
                    option.Checked = !option.Checked;

                    if (option.Checked)
                    {
                        selections.Add(option);
                        OnChangeEvent?.Invoke(selections.Select(e => e.Value));
                        continue;
                    }

                    selections.Remove(option);
                    OnChangeEvent?.Invoke(selections.Select(e => e.Value));
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (optionItems.Any(e => e.Checked))
                    {
                        selected = true;
                        continue;
                    }

                    if (defaultItems is not null)
                    {
                        selections = defaultItems.ToHashSet();
                        selected = true;
                        continue;
                    }

                    if (!required)
                    {
                        selected = true;
                        continue;
                    }

                    writer.ValidationErrors = ["This field is required."];
                }
            }

            var selectedValues = selections.Select(e => e.Value).ToArray();

            OnConfirmEvent?.Invoke(selectedValues);
            return new MultiSelectAnswer<TFieldType>(this, title, selectedValues);
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetEvent?.Invoke([]);
                return;
            }

            if (answer is MultiSelectAnswer<TFieldType> a)
            {
                OnResetEvent?.Invoke(a.Selections);
                return;
            }

            throw new ArgumentException("Wrong answer type", nameof(answer));
        }

        private bool ShouldSetValue(bool required, IEnumerable<TFieldType> initialValues, HashSet<TFieldType> possibilitiesHash)
        {
            if (GetValueToSetEvent is null)
                return false;

            if (required && initialValues.IsNullOrEmpty())
                return false;

            return initialValues.IsNullOrEmpty() || initialValues.All(possibilitiesHash.Contains);
        }

        public MultiSelect<TFieldType> Options(Func<IEnumerable<TFieldType>> getOptions)
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

        public MultiSelect<TFieldType> Options(IEnumerable<TFieldType> options)
        {
            EmptyOptionsException.ThrowIfNullOrEmpty(options);
            DuplicateOptionException.ThrowIfDuplicated(options);
            return Options(() => options);
        }

        public MultiSelect<TFieldType> Set(Func<IEnumerable<TFieldType>> getValues)
        {
            ArgumentNullException.ThrowIfNull(getValues, nameof(getValues));
            GetValueToSetEvent += getValues;
            return this;
        }

        public MultiSelect<TFieldType> Set(IEnumerable<TFieldType> values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));
            return Set(() => values);
        }

        public MultiSelect<TFieldType> Selected(Func<IEnumerable<TFieldType>> getValues)
        {
            ArgumentNullException.ThrowIfNull(getValues, nameof(getValues));
            GetSelectedValueEvent += getValues;
            return this;
        }

        public MultiSelect<TFieldType> Selected(IEnumerable<TFieldType> values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));
            return Selected(() => values);
        }

        public MultiSelect<TFieldType> OnChange(Action<IEnumerable<TFieldType>> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnChangeEvent += callback;
            return this;
        }

        public MultiSelect<TFieldType> OnConfirm(Action<IEnumerable<TFieldType>> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnConfirmEvent += callback;
            return this;
        }

        public MultiSelect<TFieldType> OnReset(Action<IEnumerable<TFieldType>> callback)
        {
            ArgumentNullException.ThrowIfNull(callback, nameof(callback));
            OnResetEvent += callback;
            return this;
        }

        public override MultiSelect<TFieldType> ValidateField()
        {
            base.ValidateField();
            OptionsNotSetException.ThrowIfNull(GetOptionsEvent);
            return this;
        }

        public MultiSelect<TFieldType> Default(Func<IEnumerable<TFieldType>> getValues)
        {
            ArgumentNullException.ThrowIfNull(getValues, nameof(getValues));
            GetDefaultValueEvent += getValues;
            return this;
        }

        public MultiSelect<TFieldType> Default(IEnumerable<TFieldType> values)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));
            return Default(() => values);
        }
    }
}
