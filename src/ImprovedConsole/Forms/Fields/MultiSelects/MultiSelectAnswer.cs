using System.Text;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer<TFieldType>(
        MultiSelect<TFieldType> multiSelect,
        string title,
        IEnumerable<TFieldType> selections) : IFieldAnswer
    {
        public IField Field => multiSelect;
        public IEnumerable<TFieldType> Selections { get; } = selections;

        public bool Equals(IFieldAnswer? other)
        {
            if (other is not MultiSelectAnswer<TFieldType> select)
                return false;

            if (Selections.Count() != select.Selections.Count())
                return false;

            var hash = Selections.ToHashSet();
            return select.Selections.All(hash.Contains);
        }

        StringBuilder IFieldAnswer.GetFormattedAnswer(int leftSpacing, FormOptions options)
        {
            List<string> values = new(4);

            int count = Selections.Count();

            var maxSize = 80;
            if (ConsoleWriter.CanGetWindowWidth())
                maxSize = ConsoleWriter.GetWindowWidth() - 10;

            var pickedItems = 0;
            var currentSize = 0;
            foreach (var item in Selections)
            {
                var strValue = multiSelect.ConvertToStringDelegate(item);
                if (strValue.Length + currentSize <= maxSize)
                {
                    values.Add(strValue);
                    pickedItems++;
                    currentSize += strValue.Length;
                    continue;
                }

                var charsToTake = maxSize - currentSize;
                if(charsToTake + 3 >= strValue.Length)
                {
                    values.Add(strValue);
                    pickedItems++;
                    break;
                }

                if (charsToTake < 5)
                    break;

                values.Add($"{strValue[..charsToTake]}...");
                pickedItems++;
                break;
            }

            if (count > pickedItems)
                values.Add($"+{count - pickedItems}");

            string answer = string.Join(", ", values);

            if (answer.Length is 0)
                answer = "N/A";

            string? formattedTitle = Message.RemoveColors(title);

            return new StringBuilder()
                .AppendLine($"{{color:{options.TitleColor}}}{formattedTitle}")
                .Append(' ', leftSpacing)
                .Append($"{{color:{options.AnswerColor}}}{answer}");
        }
    }
}
