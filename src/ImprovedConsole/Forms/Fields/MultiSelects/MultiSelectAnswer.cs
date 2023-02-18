namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer : IFieldAnswer
    {
        private readonly MultiSelect multiSelect;
        private readonly IEnumerable<PossibilityItem> selectedItems;

        public MultiSelectAnswer(MultiSelect multiSelect, IEnumerable<PossibilityItem> selectedItems)
        {
            this.multiSelect = multiSelect;
            this.selectedItems = selectedItems;
        }

        public IField Field => multiSelect;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            const int maxChars = 15;
            const int maxItems = 3;

            var values = new List<string>(4);

            var count = selectedItems.Count();
            var firstThree = selectedItems.Take(3);

            foreach (var item in firstThree)
            {
                if (item.Value.Length > maxChars)
                {
                    values.Add($"{item.Value[..maxChars]}...");
                    continue;
                }

                values.Add(item.Value);
            }

            if (count > maxItems)
                values.Add($"+{count - maxItems}");

            var answer = string.Join(", ", values);

            if (answer.Length is 0)
                answer = "No value selected";

            var title = Message.RemoveColors(multiSelect.Title);

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
