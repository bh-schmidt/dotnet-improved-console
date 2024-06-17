namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer<TFieldType>(
        MultiSelect<TFieldType> multiSelect,
        string title,
        IEnumerable<TFieldType> selections) : IFieldAnswer
    {
        public IField Field => multiSelect;
        public IEnumerable<TFieldType> Selections { get; } = selections;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            const int maxChars = 13;
            const int maxItems = 3;

            List<string> values = new(4);

            int count = Selections.Count();
            var firstThree = Selections.Take(3);

            foreach (var item in firstThree)
            {
                var strValue = item!.ToString()!;
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

            return
$@"{{color:{options.TitleColor}}}{formattedTitle}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
