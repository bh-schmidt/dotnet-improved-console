namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectAnswer<TFieldType>(SingleSelect<TFieldType> singleSelect, string title, TFieldType? selection) : IFieldAnswer
    {
        public IField Field => singleSelect;
        public TFieldType? Selection { get; set; } = selection;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Selection?.ToString() ?? "N/A";

            return
@$"{{color:{options.TitleColor}}}{formattedTitle}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
