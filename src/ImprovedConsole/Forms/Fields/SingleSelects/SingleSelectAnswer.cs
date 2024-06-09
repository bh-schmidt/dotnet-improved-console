namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectAnswer : IFieldAnswer
    {
        private readonly SingleSelect singleSelect;

        public SingleSelectAnswer(SingleSelect singleSelect, PossibilityItem? selection)
        {
            this.singleSelect = singleSelect;
            Selection = selection;
        }

        public IField Field => singleSelect;
        public PossibilityItem? Selection { get; set; }

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(singleSelect.Title);
            var answer = Selection?.Value ?? "No value selected";

            return
@$"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
