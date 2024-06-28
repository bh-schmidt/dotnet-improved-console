using ImprovedConsole.Extensions;

namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    internal class Writer<TFieldType>(
        SingleSelect<TFieldType> singleSelect,
        string title,
        List<OptionItem<TFieldType>> optionItems)
    {
        private const char CurrentRow = '>';
        private const char EmptyChar = ' ';
        private const char SelectedRow = '*';

        private int lastRow = 0;

        public IEnumerable<string>? ValidationErrors { get; set; } = [];

        public void Print(int currentIndex, int top)
        {
            bool visibility = ConsoleWriter.CanSetCursorVisibility() && ConsoleWriter.GetCursorVisibility();

            if (ConsoleWriter.CanSetCursorVisibility())
                ConsoleWriter.SetCursorVisibility(false);

            if (ConsoleWriter.CanSetCursorPosition())
                ConsoleWriter.SetCursorPosition(0, top);

            Message.WriteLine(title);

            for (int index = 0; index < optionItems.Count; index++)
            {
                var option = optionItems[index];

                ConsoleWriter.Write(' ');
                WriteCurrentIcon(index, currentIndex);
                ConsoleWriter.Write(" [");
                WriteSelectedIcon(option);
                ConsoleWriter.Write("] ");

                ConsoleWriter.WriteLine(singleSelect.ConvertToStringDelegate(option.Value));
            }

            if (ConsoleWriter.CanSetCursorPosition())
            {
                var errorRow = ConsoleWriter.GetCursorPosition().Top;
                if (lastRow - errorRow >= 0)
                    ConsoleWriter.ClearLines(errorRow, lastRow);
            }

            if (!ValidationErrors.IsNullOrEmpty())
            {
                foreach (string item in ValidationErrors)
                {
                    Message.WriteLine("{color:red} * " + item);
                }
            }

            ConsoleWriter.WriteLine("press <h> for help");

            lastRow = ConsoleWriter.GetCursorPosition().Top;

            if (ConsoleWriter.CanSetCursorVisibility())
                ConsoleWriter.SetCursorVisibility(visibility);
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
