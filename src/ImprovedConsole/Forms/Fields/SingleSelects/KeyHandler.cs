namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    internal class KeyHandler
    {
        private readonly SingleSelect select;
        private readonly Writer writer;

        public KeyHandler(
            SingleSelect select,
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
            UncheckOthers(currentIndex);

            select.OnChangeAction(possibility);
        }

        public SingleSelectAnswer? HandleEnter(ConsoleKeyInfo key)
        {
            if (key.Key != ConsoleKey.Enter)
                return null;

            if (!Validate())
                return null;

            var selection = select
                .Possibilities
                .Where(e => e.Checked)
                .FirstOrDefault();

            select.OnConfirmAction(selection);

            return new SingleSelectAnswer(select, selection);
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

            if (select.Options.Required)
            {
                possibility.Checked = true;
                writer.SetNewSelection(possibility);
                return;
            }

            possibility.Checked = !possibility.Checked;
            if (possibility.Checked)
            {
                writer.SetNewSelection(possibility);
                return;
            }

            writer.ClearOldSelection(possibility);
        }

        private void UncheckOthers(int currentIndex)
        {
            for (int i = 0; i < select.Possibilities.Length; i++)
            {
                if (currentIndex == i)
                    continue;

                var possibility = select.Possibilities[i];
                possibility.Checked = false;
                writer.ClearOldSelection(possibility);
            }
        }

        private bool Validate()
        {
            if (select.Options.Required && !select.Possibilities.Any(e => e.Checked))
            {
                select.Error = SingleSelectErrorEnum.SelectionIsRequired;
                return false;
            }

            return true;
        }
    }
}
