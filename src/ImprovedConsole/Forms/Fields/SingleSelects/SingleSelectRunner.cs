namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectRunner<TFieldType>(
        SingleSelect<TFieldType> singleSelect,
        string title,
        bool required,
        List<OptionItem<TFieldType>> optionItems,
        OptionItem<TFieldType>? defaultItem,
        OptionItem<TFieldType>? initialSelection)
    {
        public SingleSelectAnswer<TFieldType> Run()
        {
            int currentIndex = 0;
            bool selected = false;

            OptionItem<TFieldType>? selection = initialSelection;
            if (initialSelection is not null)
                currentIndex = optionItems.IndexOf(initialSelection);

            var writer = new Writer<TFieldType>(
                singleSelect,
                title,
                optionItems);

            singleSelect.FormEvents.Reprint();
            var (Left, Top) = ConsoleWriter.GetCursorPosition();

            var firstPrint = true;
            while (!selected)
            {
                if (!ConsoleWriter.CanSetCursorPosition() && !firstPrint)
                    singleSelect.FormEvents.Reprint();

                writer.Print(currentIndex, Top);
                firstPrint = false;

                ConsoleKeyInfo key = ConsoleWriter.ReadKey(true);

                if(key.Key == ConsoleKey.H)
                {
                    SelectHelp.Print();
                    singleSelect.FormEvents.Reprint();
                    writer.Print(currentIndex, Top);
                    continue;
                }

                if (key.Key is ConsoleKey.DownArrow or ConsoleKey.J)
                {
                    currentIndex++;

                    if (currentIndex >= optionItems.Count)
                        currentIndex = 0;

                    if (required)
                        selection = Select(currentIndex);

                    continue;
                }

                if (key.Key is ConsoleKey.UpArrow or ConsoleKey.K)
                {
                    currentIndex--;

                    if (currentIndex < 0)
                        currentIndex = optionItems.Count - 1;

                    if (required)
                        selection = Select(currentIndex);

                    continue;
                }

                if (key.Key == ConsoleKey.Spacebar)
                {
                    selection = Toggle(currentIndex);
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (selection?.Checked == true)
                    {
                        selected = true;
                        continue;
                    }

                    if (required)
                    {
                        writer.ValidationErrors = ["This field is required."];
                        continue;
                    }

                    if (defaultItem is not null)
                    {
                        selection = defaultItem;
                        selected = true;
                        continue;
                    }

                    selected = true;
                }
            }

            if (selection is null)
            {
                singleSelect.OnConfirmEvent?.Invoke(default);
                return new SingleSelectAnswer<TFieldType>(singleSelect, title, default);
            }

            singleSelect.OnConfirmEvent?.Invoke(selection.Value);
            return new SingleSelectAnswer<TFieldType>(singleSelect, title, selection.Value);
        }

        private OptionItem<TFieldType>? Toggle(int currentIndex)
        {
            var selection = optionItems[currentIndex];
            if (!selection.Checked)
                return Select(currentIndex);

            if (required)
                return selection;

            selection.Checked = false;
            singleSelect.OnChangeEvent?.Invoke(default);
            return null;
        }

        private OptionItem<TFieldType>? Select(int currentIndex)
        {
            var selection = optionItems[currentIndex];
            selection.Checked = true;

            singleSelect.OnChangeEvent?.Invoke(selection.Value);
            UnselectOthers(currentIndex);
            return selection;
        }

        private void UnselectOthers(int currentIndex)
        {
            for (int i = 0; i < optionItems.Count; i++)
            {
                if (i != currentIndex)
                    optionItems[i].Checked = false;
            }
        }
    }
}
