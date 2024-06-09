namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class MultiSelectAnswer : IFieldAnswer
    {
        private readonly MultiSelect multiSelect;

        public MultiSelectAnswer(MultiSelect multiSelect, IEnumerable<PossibilityItem> selectedItems)
        {
            this.multiSelect = multiSelect;
            SelectedItems = selectedItems;
        }

        public IField Field => multiSelect;
        public IEnumerable<PossibilityItem> SelectedItems { get; }

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            const int maxChars = 15;
            const int maxItems = 3;

            var values = new List<string>(4);

            var count = SelectedItems.Count();
            var firstThree = SelectedItems.Take(3);

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
