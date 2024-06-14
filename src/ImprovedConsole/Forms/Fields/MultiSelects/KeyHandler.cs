namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    internal class KeyHandler(
        MultiSelect select)
    {
        private readonly MultiSelect select = select;

        public void HandleKey(ref int currentIndex, ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.DownArrow)
            {
                IncrementPosition(ref currentIndex);
                return;
            }

            if (key.Key == ConsoleKey.UpArrow)
            {
                DecrementPosition(ref currentIndex);
                return;
            }

            if (key.Key == ConsoleKey.Spacebar)
            {
                HandleCheck(currentIndex);
                return;
            }
        }

        private void HandleCheck(int currentIndex)
        {
            PossibilityItem possibility = select.Possibilities[currentIndex];
            CheckPossibility(possibility);

            select.OnChangeAction(possibility);
        }

        public MultiSelectAnswer? GetAnswer(ConsoleKeyInfo key)
        {
            if (key.Key != ConsoleKey.Enter)
                return null;

            if (!Validate())
                return null;

            IEnumerable<PossibilityItem> selections = select
                .Possibilities
                .Where(e => e.Checked);

            select.OnConfirmAction(selections);

            return new MultiSelectAnswer(select, selections);
        }

        private void IncrementPosition(ref int currentIndex)
        {
            PossibilityItem possibility = select.Possibilities[currentIndex];
            Writer.ClearCurrentPosition(possibility);

            currentIndex++;

            if (currentIndex >= select.Possibilities.Length)
                currentIndex = 0;

            possibility = select.Possibilities[currentIndex];
            Writer.SetNewPosition(possibility);
        }

        private void DecrementPosition(ref int currentIndex)
        {
            PossibilityItem possibility = select.Possibilities[currentIndex];
            Writer.ClearCurrentPosition(possibility);

            currentIndex--;

            if (currentIndex < 0)
                currentIndex = select.Possibilities.Length - 1;

            possibility = select.Possibilities[currentIndex];
            Writer.SetNewPosition(possibility);
        }

        private static void CheckPossibility(PossibilityItem possibility)
        {
            possibility.Checked = !possibility.Checked;
            if (possibility.Checked)
            {
                Writer.SetNewSelection(possibility);
                return;
            }

            Writer.ClearOldSelection(possibility);
        }

        private bool Validate()
        {
            if (select.Options.Required && !select.Possibilities.Any(e => e.Checked))
            {
                select.Error = MultiSelectErrorEnum.SelectionIsRequired;
                return false;
            }

            return true;
        }
    }
}
