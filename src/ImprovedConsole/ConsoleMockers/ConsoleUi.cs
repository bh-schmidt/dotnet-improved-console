using System.Text;
using System.Text.RegularExpressions;

namespace ImprovedConsole.ConsoleMockers
{
    public partial class ConsoleUi
    {
        private const char NewLineChar = '\n';
        private const char DefaultChar = ' ';
        private readonly int windowWidth = 120;
        private int left = 0;
        private int top = 0;
        private List<List<char>> ui;

        public ConsoleUi()
        {
            ui = [];
            AddRow();
        }

        public void Write(object? value)
        {
            string? str = value?.ToString();
            if (string.IsNullOrEmpty(str))
                return;

            string[] split = NewLineRegex().Split(str);
            EnsureRows(split.Length);

            for (int index = 0; index < split.Length; index++)
            {
                string newString = split[index];
                Append(newString);

                if (index + 1 != split.Length)
                    AddNewLine();
            }
        }

        public void Clear()
        {
            ui = [];
            left = 0;
            top = 0;
            AddRow();
        }

        public (int Left, int Top) GetCursorPosition()
        {
            return (left, top);
        }

        public string GetOutput()
        {
            StringBuilder builder = new();

            for (int lineIndex = 0; lineIndex < ui.Count; lineIndex++)
            {
                int lastValidChar = windowWidth - 1;
                List<char> row = ui[lineIndex];

                if (row[windowWidth] == NewLineChar || ui.Count == lineIndex + 1)
                    lastValidChar = GetLastValidChar(row);

                for (int rowIndex = 0; rowIndex <= lastValidChar; rowIndex++)
                    builder.Append(row[rowIndex]);

                if (lineIndex < ui.Count - 1)
                    builder.AppendLine();
            }

            return builder.ToString();
        }

        public int GetWindowWidth() => windowWidth;

        public void SetCursorPosition(int left, int top)
        {
            if (left >= windowWidth)
                throw new IndexOutOfRangeException($"The left position must be smaller than the window width");

            this.left = left;
            this.top = top;
        }

        public void WriteLine()
        {
            AddNewLine();
        }

        public void WriteLine(object? value)
        {
            Write(value);
            AddNewLine();
        }

        private void AddNewLine()
        {
            ui[top][windowWidth] = NewLineChar;
            top++;
            left = 0;
            if (top == ui.Count)
                AddRow();
        }

        private void EnsureRows(int count)
        {
            int minimumCount = top + Math.Max(count, 0);
            ui.EnsureCapacity(minimumCount);
            while (ui.Count < minimumCount)
                AddRow();
        }

        private void Append(string newString)
        {
            foreach (char value in newString)
            {
                if (left == windowWidth)
                {
                    top++;
                    left = 0;
                    EnsureRows(1);
                }

                ui[top][left++] = value;
            }
        }

        private void AddRow()
        {
            int rowSize = windowWidth + 1;
            List<char> row = new(rowSize);
            ui.Add(row);

            for (int i = 0; i < rowSize; i++)
                row.Add(DefaultChar);
        }

        private int GetLastValidChar(List<char> row)
        {
            for (int index = windowWidth - 1; index >= 0; index--)
                if (row[index] is not DefaultChar)
                    return index;

            return -1;
        }

        [GeneratedRegex(@"\r\n|\n|\r", RegexOptions.Compiled)]
        private static partial Regex NewLineRegex();
    }
}
