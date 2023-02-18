namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectAnswer : IFieldAnswer
    {
        private readonly SingleSelect singleSelect;
        private readonly PossibilityItem? selection;

        public SingleSelectAnswer(SingleSelect singleSelect, PossibilityItem? selection)
        {
            this.singleSelect = singleSelect;
            this.selection = selection;
        }

        public IField Field => singleSelect;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(singleSelect.Title);
            var answer = selection?.Value ?? "No value selected";

            return
@$"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
