namespace ImprovedConsole.Forms.Fields.DecimalFields
{
    public class DecimalFieldAnswer(DecimalField textField, decimal? answer) : IFieldAnswer
    {
        private readonly DecimalField textField = textField;

        public IField Field => textField;
        public decimal? Answer { get; } = answer;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? title = Message.RemoveColors(textField.Title);
            string answer = this.Answer?.ToString() ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
