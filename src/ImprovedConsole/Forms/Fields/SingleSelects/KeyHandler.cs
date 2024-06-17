namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    internal class KeyHandler<TFieldType>(
        bool required,
        OptionItem<TFieldType>[] optionItems)
    {
        public OptionItem<TFieldType> HandleCheck(int currentIndex)
        {
            var option = optionItems[currentIndex];
            CheckOption(option);
            UncheckOthers(currentIndex);
            return option;
        }

        public void IncrementPosition(ref int currentIndex)
        {
            currentIndex++;

            if (currentIndex >= optionItems.Length)
                currentIndex = 0;
        }

        public void DecrementPosition(ref int currentIndex)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = optionItems.Length - 1;
        }

        private void CheckOption(OptionItem<TFieldType> option)
        {
            if (required)
            {
                option.Checked = true;
                return;
            }

            option.Checked = !option.Checked;
        }

        private void UncheckOthers(int currentIndex)
        {
            for (int i = 0; i < optionItems.Length; i++)
            {
                if (currentIndex == i)
                    continue;

                var option = optionItems[i];
                option.Checked = false;
            }
        }
    }
}
