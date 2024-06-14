namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer(MultiSelect multiSelect, IEnumerable<PossibilityItem> selectedItems) : IFieldAnswer
    {
        private readonly MultiSelect multiSelect = multiSelect;

        public IField Field => multiSelect;
        public IEnumerable<PossibilityItem> SelectedItems { get; } = selectedItems;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            const int maxChars = 15;
            const int maxItems = 3;

            List<string> values = new List<string>(4);

            int count = SelectedItems.Count();
            IEnumerable<PossibilityItem> firstThree = SelectedItems.Take(3);

            foreach (PossibilityItem? item in firstThree)
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

            string answer = string.Join(", ", values);

            if (answer.Length is 0)
                answer = "No value selected";

            string? title = Message.RemoveColors(multiSelect.Title);

            return
$@"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
