namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectAnswer(SingleSelect singleSelect, PossibilityItem? selection) : IFieldAnswer
    {
        private readonly SingleSelect singleSelect = singleSelect;

        public IField Field => singleSelect;
        public PossibilityItem? Selection { get; set; } = selection;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? title = Message.RemoveColors(singleSelect.Title);
            string answer = Selection?.Value ?? "No value selected";

            return
@$"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
