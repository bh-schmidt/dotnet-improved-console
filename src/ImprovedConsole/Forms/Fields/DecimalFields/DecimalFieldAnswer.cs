namespace ImprovedConsole.Forms.Fields.DecimalFields
{
    public class DecimalFieldAnswer : IFieldAnswer
    {
        private readonly DecimalField textField;
        private readonly decimal? answer;

        public DecimalFieldAnswer(DecimalField textField, decimal? answer)
        {
            this.textField = textField;
            this.answer = answer;
        }

        public IField Field => textField;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textField.Title);
            var answer = this.answer?.ToString() ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
