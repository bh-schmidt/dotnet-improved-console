namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldAnswer<TField>(
        TextField<TField> textField,
        string title,
        TField? answer) : IFieldAnswer
    {
        private readonly TextField<TField> textField = textField;

        public IField Field => textField;
        public TField? Answer { get; set; } = answer;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Answer?.ToString() ?? "N/A";

            return
$@"{{color:{options.TitleColor}}}{formattedTitle}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
