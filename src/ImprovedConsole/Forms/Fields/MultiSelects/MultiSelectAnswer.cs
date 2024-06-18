using System.Text;

namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer<TFieldType>(
        MultiSelect<TFieldType> multiSelect,
        string title,
        IEnumerable<TFieldType> selections,
        Func<TFieldType, string> convertToString) : IFieldAnswer
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
            const int maxChars = 13;
            const int maxItems = 3;

            List<string> values = new(4);

            int count = Selections.Count();
            var firstThree = Selections.Take(3);

            foreach (var item in firstThree)
            {
                var strValue = convertToString(item);
                if (strValue.Length > maxChars)
                {
                    var charsToTake = maxChars - 3;
                    values.Add($"{strValue[..charsToTake]}...");
                    continue;
                }

                values.Add(strValue);
            }

            if (count > maxItems)
                values.Add($"+{count - maxItems}");

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
