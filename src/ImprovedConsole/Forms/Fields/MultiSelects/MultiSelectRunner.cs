using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectRunner<TFieldType>(
        MultiSelect<TFieldType> multiSelect,
        string title,
        bool required,
        List<OptionItem<TFieldType>> optionItems,
        IEnumerable<OptionItem<TFieldType>>? defaultItems,
        HashSet<OptionItem<TFieldType>>? initialSelections)
    {
        public MultiSelectAnswer<TFieldType> Run()
        {
            int currentIndex = 0;
            bool selected = false;

            var selections = initialSelections ?? [];
            if (initialSelections is not null)
                currentIndex = optionItems.IndexOf(initialSelections.First());

            var writer = new Writer<TFieldType>(multiSelect, title, optionItems);

            multiSelect.FormEvents.Reprint();
            (_, int Top) = ConsoleWriter.GetCursorPosition();

            var firstPrint = true;
            while (!selected)
            {
                if (!ConsoleWriter.CanSetCursorPosition() && !firstPrint)
                    multiSelect.FormEvents.Reprint();

                writer.Print(currentIndex, Top);
                firstPrint = false;

                ConsoleKeyInfo key = ConsoleWriter.ReadKey(true);

                if (key.Key == ConsoleKey.H)
                {
                    SelectHelp.Print();
                    multiSelect.FormEvents.Reprint();
                    writer.Print(currentIndex, Top);
                    continue;
                }

                if (key.Key is ConsoleKey.DownArrow or ConsoleKey.J)
                {
                    currentIndex++;

                    if (currentIndex >= optionItems.Count)
                        currentIndex = 0;

                    continue;
                }

                if (key.Key is ConsoleKey.UpArrow or ConsoleKey.K)
                {
                    currentIndex--;

                    if (currentIndex < 0)
                        currentIndex = optionItems.Count - 1;

                    continue;
                }

                if (key.Key == ConsoleKey.Spacebar)
                {
                    var option = optionItems[currentIndex];

                    if (option.Checked)
                    {
                        option.Checked = false;
                        selections.Remove(option);
                        multiSelect.OnChangeEvent?.Invoke(selections.Select(e => e.Value));
                        continue;
                    }

                    option.Checked = true;
                    selections.Add(option);
                    multiSelect.OnChangeEvent?.Invoke(selections.Select(e => e.Value));
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (optionItems.Any(e => e.Checked))
                    {
                        selected = true;
                        continue;
                    }

                    if (required)
                    {
                        writer.ValidationErrors = ["This field is required."];
                        continue;
                    }

                    if (defaultItems is not null)
                    {
                        selections = defaultItems.ToHashSet();
                        selected = true;
                        continue;
                    }

                    selected = true;
                    continue;
                }
            }

            var selectedValues = optionItems
                .Where(selections.Contains)
                .Select(e => e.Value)
                .ToArray();

            multiSelect.OnConfirmEvent?.Invoke(selectedValues);
            return new MultiSelectAnswer<TFieldType>(multiSelect, title, selectedValues);
        }
    }
}
