namespace ImprovedConsole.Forms.Fields.DecimalFields
{
    public class DecimalFieldAnswer : IFieldAnswer
    {
        private readonly DecimalField textField;

        public DecimalFieldAnswer(DecimalField textField, decimal? answer)
        {
            this.textField = textField;
            Answer = answer;
        }

        public IField Field => textField;
        public decimal? Answer { get; }

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textField.Title);
            var answer = this.Answer?.ToString() ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
