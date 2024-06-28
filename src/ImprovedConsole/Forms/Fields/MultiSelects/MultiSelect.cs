using ImprovedConsole.Extensions;
using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelect<TFieldType>(FormEvents formEvents) :
        Field<TFieldType, MultiSelect<TFieldType>>,
        IField,
        IOptions<TFieldType, MultiSelect<TFieldType>>,
        ISetValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        ISelectedValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IDefaultValue<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnChange<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnConfirm<IEnumerable<TFieldType>, MultiSelect<TFieldType>>,
        IOnReset<IEnumerable<TFieldType>, MultiSelect<TFieldType>>
    {
        internal FormEvents FormEvents { get; } = formEvents;

        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; private set; }
        public Func<IEnumerable<TFieldType>>? GetExternalValueEvent { get; private set; }
        public Func<IEnumerable<TFieldType>>? GetSelectedValueEvent { get; private set; }
        public Action<IEnumerable<TFieldType>>? OnChangeEvent { get; private set; }
        public Action<IEnumerable<TFieldType>>? OnConfirmEvent { get; private set; }
        public Action<IEnumerable<TFieldType>>? OnResetEvent { get; private set; }
        public Func<IEnumerable<TFieldType>>? GetDefaultValueEvent { get; private set; }

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

        private MultiSelectAnswer<TFieldType> RunInternal()
        {
            var title = GetTitle();
            var required = IsRequired();
            var options = GetOptionsEvent!().ToList();
            var optionItems = options.Select(e => new OptionItem<TFieldType>(e)).ToList();

            var externalValues = GetExternalValue(title, required, options);
            if (externalValues is not null)
                return externalValues;

            var defaultItems = GetDefaultItems(options, optionItems, required);
            var selections = GetSelections(options, optionItems);

            var runner = new MultiSelectRunner<TFieldType>(
                this,
                title,
                required,
                optionItems,
                defaultItems,
                selections);

            return runner.Run();
        }

        private HashSet<OptionItem<TFieldType>>? GetSelections(List<TFieldType> options, List<OptionItem<TFieldType>> optionItems)
        {
            var optionsSet = options.ToHashSet();

            if (Answer is not null)
            {
                var answer = (MultiSelectAnswer<TFieldType>)Answer;

                if (answer is not null && answer.Selections.All(optionsSet.Contains))
                {
                    var anserSet = answer.Selections.ToHashSet();

                    var selections = optionItems
                        .Where(e => anserSet.Contains(e.Value))
                        .ToHashSet();

                    foreach (var item in selections)
                        item.Checked = true;

                    return selections;
                }
            }

            var selectedOptions = (GetSelectedValueEvent?.Invoke() ?? [])
                .ToHashSet();

            if (GetSelectedValueEvent is not null && !selectedOptions.IsNullOrEmpty())
            {
                OptionNotAllowedException.ThrowIfInvalid(selectedOptions, optionsSet);

                var selections = optionItems
                    .Where(e => selectedOptions.Contains(e.Value))
                    .ToHashSet();

                foreach (var item in selections)
                    item.Checked = true;

                return selections;
            }

            return null;
        }

        private IEnumerable<OptionItem<TFieldType>>? GetDefaultItems(List<TFieldType> options, List<OptionItem<TFieldType>> optionItems, bool required)
        {
            if (GetDefaultValueEvent is null)
                return null;

            if (required)
                return null;

            var defaultValues = GetDefaultValueEvent?.Invoke() ?? [];
            if (!defaultValues.Any())
                return null;

            OptionNotAllowedException.ThrowIfInvalid(defaultValues, options);

            var set = defaultValues.ToHashSet();
            return optionItems.Where(e => set.Contains(e.Value));
        }

        private MultiSelectAnswer<TFieldType>? GetExternalValue(string title, bool required, List<TFieldType> options)
        {
            if (GetExternalValueEvent is null)
                return null;


            if (IsEditing)
                return null;

            if (GetExternalValueEvent is null)
                return null;

            var externalValues = GetExternalValueEvent();
            if (required && externalValues.IsNullOrEmpty())
                return null;

            var possibilitiesHash = options.ToHashSet();
            if (externalValues.IsNullOrEmpty() || externalValues.All(possibilitiesHash.Contains))
            {
                var answer = new MultiSelectAnswer<TFieldType>(
                    this,
                    title,
                    externalValues);
                OnConfirmEvent?.Invoke(externalValues);
                return answer;
            }

            return null;
        }

        public override void Reset()
        {
            var answer = Answer as MultiSelectAnswer<TFieldType>;

            Answer = null;
            Finished = false;

            if (answer is null)
            {
                OnResetEvent?.Invoke([]);
                return;
            }

            OnResetEvent?.Invoke(answer!.Selections);
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
            GetExternalValueEvent += getValues;
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
