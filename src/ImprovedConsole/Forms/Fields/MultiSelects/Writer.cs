using ImprovedConsole.Extensions;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    internal class Writer<TFieldType>(string title, OptionItem<TFieldType>[] optionItems, Func<TFieldType, string> convertToString)
    {
        private const char CurrentRow = '>';
        private const char EmptyChar = ' ';
        private const char SelectedRow = 'x';

        private int lastErrorRow = 0;

        public IEnumerable<string>? ValidationErrors { get; set; } = [];

        public void Print(int currentIndex, int top)
        {
            if (ConsoleWriter.CanSetCursorPosition())
                ConsoleWriter.SetCursorPosition(0, top);

            Message.WriteLine(title);

            for (int index = 0; index < optionItems.Length; index++)
            {
                var option = optionItems[index];
                (_, int Top) = ConsoleWriter.GetCursorPosition();
                option.Position = Top;

                ConsoleWriter.Write(' ');
                WriteCurrentIcon(index, currentIndex);
                ConsoleWriter.Write(" [");
                WriteSelectedIcon(option);
                ConsoleWriter.Write("] ");

                ConsoleWriter.WriteLine(convertToString(option.Value));
            }

            if (ConsoleWriter.CanSetCursorPosition())
            {
                var errorRow = ConsoleWriter.GetCursorPosition().Top;
                if (lastErrorRow - errorRow >= 0)
                    ConsoleWriter.ClearLines(errorRow, lastErrorRow);
            }

            if (!ValidationErrors.IsNullOrEmpty())
            {
                foreach (string item in ValidationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }
        }

        private static void WriteSelectedIcon(OptionItem<TFieldType> item)
        {
            if (item.Checked)
            {
                ConsoleWriter.Write(SelectedRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }

        private static void WriteCurrentIcon(int arrayIndex, int currentIndex)
        {
            if (arrayIndex == currentIndex)
            {
                ConsoleWriter.Write(CurrentRow);
                return;
            }

            ConsoleWriter.Write(EmptyChar);
        }
    }
}
