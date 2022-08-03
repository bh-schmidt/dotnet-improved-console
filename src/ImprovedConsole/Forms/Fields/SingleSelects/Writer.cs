namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    internal class Writer
    {
        private const char CurrentRow = '>';
        private const char EmptyChar = ' ';
        private const char SelectedRow = '*';
        private readonly SingleSelect select;

        public Writer(SingleSelect select)
        {
            this.select = select;
        }

        public void Print()
        {
            Message.WriteLine(select.Title);

            for (int index = 0; index < select.Possibilities.Length; index++)
            {
                var possibility = select.Possibilities[index];

                var currentPosition = ConsoleWriter.GetCursorPosition();
                possibility.Position = currentPosition.Top;

                ConsoleWriter.Write(' ');
                WriteCurrentIcon(index);
                ConsoleWriter.Write(" [");
                WriteSelectedIcon(possibility);
                ConsoleWriter.Write("] ");

                ConsoleWriter.WriteLine(possibility.Value);
            }
        }

        public void PrintError(ref int errorLine)
        {
            if (select.Error is not null)
            {
                ConsoleWriter.SetCursorPosition(0, errorLine);

                var message = ErrorMessages.SingleSelect(select.Error.Value);
                Message.WriteLine(message);
            }
        }

        private void WriteSelectedIcon(PossibilityItem item)
        {
            if (item.Checked)
            {
                ConsoleWriter.Write(SelectedRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }

        private void WriteCurrentIcon(int arrayIndex)
        {
            if (arrayIndex == 0)
            {
                ConsoleWriter.Write(CurrentRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }

        public void SetNewSelection(PossibilityItem possibility)
        {
            var currentPosition = ConsoleWriter.GetCursorPosition();
            var position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(4, position)
                .Write(SelectedRow)
                .SetCursorPosition(currentPosition.Left, currentPosition.Top);
        }

        public void ClearOldSelection(PossibilityItem possibility)
        {
            var currentPosition = ConsoleWriter
                .GetCursorPosition();

            var position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(4, position)
                .Write(EmptyChar)
                .SetCursorPosition(currentPosition.Left, currentPosition.Top);
        }

        public void SetNewPosition(PossibilityItem possibility)
        {
            var currentPosition = ConsoleWriter
                .GetCursorPosition();

            var position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(1, position)
                .Write(CurrentRow)
                .SetCursorPosition(currentPosition.Left, currentPosition.Top);

        }

        public void ClearCurrentPosition(PossibilityItem possibility)
        {
            var currentPosition = ConsoleWriter
                .GetCursorPosition();

            var position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(1, position)
                .Write(EmptyChar)
                .SetCursorPosition(currentPosition.Left, currentPosition.Top);
        }
    }
}
