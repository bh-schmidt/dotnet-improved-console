namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    internal class KeyHandler
    {
        private readonly MultiSelect select;
        private readonly Writer writer;

        public KeyHandler(
            MultiSelect select,
            Writer writer)
        {
            this.select = select;
            this.writer = writer;
        }

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
            var possibility = select.Possibilities[currentIndex];
            CheckPossibility(possibility);

            select.OnChangeAction(possibility);
        }

        public MultiSelectAnswer? GetAnswer(ConsoleKeyInfo key)
        {
            if (key.Key != ConsoleKey.Enter)
                return null;

            if (!Validate())
                return null;

            var selections = select
                .Possibilities
                .Where(e => e.Checked);

            select.OnConfirmAction(selections);

            return new MultiSelectAnswer(select, selections);
        }

        private void IncrementPosition(ref int currentIndex)
        {
            var possibility = select.Possibilities[currentIndex];
            writer.ClearCurrentPosition(possibility);

            currentIndex++;

            if (currentIndex >= select.Possibilities.Length)
                currentIndex = 0;

            possibility = select.Possibilities[currentIndex];
            writer.SetNewPosition(possibility);
        }

        private void DecrementPosition(ref int currentIndex)
        {
            var possibility = select.Possibilities[currentIndex];
            writer.ClearCurrentPosition(possibility);

            currentIndex--;

            if (currentIndex < 0)
                currentIndex = select.Possibilities.Length - 1;

            possibility = select.Possibilities[currentIndex];
            writer.SetNewPosition(possibility);
        }

        private void CheckPossibility(PossibilityItem possibility)
        {
            possibility.Checked = !possibility.Checked;
            if (possibility.Checked)
            {
                writer.SetNewSelection(possibility);
                return;
            }

            writer.ClearOldSelection(possibility);
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
