namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    internal class KeyHandler<TFieldType>(OptionItem<TFieldType>[] optionItems)
    {
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
    }
}
