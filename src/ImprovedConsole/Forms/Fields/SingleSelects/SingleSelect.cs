using ImprovedConsole.Forms.Exceptions;
using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelect<TFieldType>(FormEvents formEvents) :
        Field<TFieldType, SingleSelect<TFieldType>>,
        IField,
        IResettable,
        IOptions<TFieldType, SingleSelect<TFieldType>>,
        IDefaultValue<TFieldType?, SingleSelect<TFieldType>>,
        ISelectedValue<TFieldType?, SingleSelect<TFieldType>>,
        ISetValue<TFieldType?, SingleSelect<TFieldType>>,
        IOnChange<TFieldType?, SingleSelect<TFieldType>>,
        IOnConfirm<TFieldType?, SingleSelect<TFieldType>>,
        IOnReset<TFieldType?, SingleSelect<TFieldType>>
    {
        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; set; }
        public Func<TFieldType?>? GetValueToSetEvent { get; set; }
        public Action<TFieldType?>? OnConfirmEvent { get; set; }
        public Action<TFieldType?>? OnResetEvent { get; set; }
        public Action<TFieldType?>? OnChangeEvent { get; set; }
        public Func<TFieldType?>? GetSelectedValueEvent { get; set; }
        public Func<TFieldType?>? GetDefaultValueEvent { get; set; }

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

        private SingleSelectAnswer<TFieldType> RunInternal()
        {
            int currentIndex = 0;
            bool selected = false;
            OptionItem<TFieldType>? selection = null;

            var title = getTitle();
            var required = isRequired();
            var options = GetOptionsEvent!().ToList();

            var valuesToSet = GetValueToSetEvent is null ?
                default :
                GetValueToSetEvent();

            var optionItems = options
                .Select(e => new OptionItem<TFieldType>(e))
                .ToArray();

            var defaultValue = GetDefaultValueEvent is null ?
                default :
                GetDefaultValueEvent();

            var defaultItem = GetDefaultValueEvent is null ?
                null :
                new OptionItem<TFieldType>(defaultValue!);

            if (GetDefaultValueEvent is not null)
                OptionNotAllowedException.ThrowIfInvalid(defaultValue, options);

            if (ShouldSetValue(required, valuesToSet, options))
            {
                var answer = new SingleSelectAnswer<TFieldType>(this, title, valuesToSet, convertToString);
                GetValueToSetEvent = null;
                return answer;
            }

            var selectedOption = GetSelectedValueEvent is null ?
                default :
                GetSelectedValueEvent.Invoke();

            if (GetSelectedValueEvent is not null && selectedOption is not null)
            {
                OptionNotAllowedException.ThrowIfInvalid(selectedOption, options);
                var index = options.IndexOf(selectedOption);
                selection = optionItems[index];
                selection.Checked = true;
                currentIndex = index;
            }

            var writer = new Writer<TFieldType>(
                title,
                optionItems,
                convertToString);

            var keyHandler = new KeyHandler<TFieldType>(
                required,
                optionItems);

            formEvents.Reprint();
            var (Left, Top) = ConsoleWriter.GetCursorPosition();

            var firstPrint = true;
            while (!selected)
            {
                if (!ConsoleWriter.CanSetCursorPosition() && !firstPrint)
                    formEvents.Reprint();

                writer.Print(currentIndex, Top);
                firstPrint = false;

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
                    var currentSelection = keyHandler.HandleCheck(currentIndex);

                    if (!currentSelection.Checked)
                    {
                        OnChangeEvent?.Invoke(default);
                        selection = null;
                        continue;
                    }

                    if (currentSelection == selection)
                        continue;

                    OnChangeEvent?.Invoke(currentSelection.Value);
                    selection = currentSelection;
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (selection?.Checked == true)
                    {
                        selected = true;
                        continue;
                    }

                    if (defaultItem is not null)
                    {
                        selection = defaultItem;
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

            if (selection is null)
            {
                OnConfirmEvent?.Invoke(default);
                return new SingleSelectAnswer<TFieldType>(this, title, default, convertToString);
            }

            OnConfirmEvent?.Invoke(selection.Value);
            return new SingleSelectAnswer<TFieldType>(this, title, selection.Value, convertToString);
        }

        public SingleSelect<TFieldType> OnChange(Action<TFieldType?> callback)
        {
            OnChangeEvent += callback ?? throw new ArgumentNullException(nameof(callback));
            return this;
        }

        void IResettable.Reset(IFieldAnswer? answer)
        {
            if (answer == null)
            {
                OnResetEvent?.Invoke(default);
                return;
            }

            if (answer is SingleSelectAnswer<TFieldType> a)
            {
                if (a.Selection is null)
                {
                    OnResetEvent?.Invoke(default);
                    return;
                }

                OnResetEvent?.Invoke(a.Selection);
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
            GetValueToSetEvent += getValue;
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
