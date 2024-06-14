namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    internal class Writer(MultiSelect select)
    {
        private const char CurrentRow = '>';
        private const char EmptyChar = ' ';
        private const char SelectedRow = 'x';
        private readonly MultiSelect select = select;

        public void Print()
        {
            Message.WriteLine(select.Title);

            for (int index = 0; index < select.Possibilities.Length; index++)
            {
                PossibilityItem possibility = select.Possibilities[index];
                (_, int Top) = ConsoleWriter.GetCursorPosition();
                possibility.Position = Top;

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

                string message = ErrorMessages.MultiSelect(select.Error.Value);
                Message.WriteLine(message);
            }
        }

        private static void WriteSelectedIcon(PossibilityItem item)
        {
            if (item.Checked)
            {
                ConsoleWriter.Write(SelectedRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }

        private static void WriteCurrentIcon(int arrayIndex)
        {
            if (arrayIndex == 0)
            {
                ConsoleWriter.Write(CurrentRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }

        public static void SetNewSelection(PossibilityItem possibility)
        {
            (int Left, int Top) = ConsoleWriter.GetCursorPosition();
            int position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(4, position)
                .Write(SelectedRow)
                .SetCursorPosition(Left, Top);
        }

        public static void ClearOldSelection(PossibilityItem possibility)
        {
            (int Left, int Top) = ConsoleWriter
                .GetCursorPosition();

            int position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(4, position)
                .Write(EmptyChar)
                .SetCursorPosition(Left, Top);
        }

        public static void SetNewPosition(PossibilityItem possibility)
        {
            (int Left, int Top) = ConsoleWriter
                .GetCursorPosition();

            int position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(1, position)
                .Write(CurrentRow)
                .SetCursorPosition(Left, Top);
        }

        public static void ClearCurrentPosition(PossibilityItem possibility)
        {
            (int Left, int Top) = ConsoleWriter
                .GetCursorPosition();

            int position = possibility.Position;

            ConsoleWriter
                .SetCursorPosition(1, position)
                .Write(EmptyChar)
                .SetCursorPosition(Left, Top);
        }
    }
}
