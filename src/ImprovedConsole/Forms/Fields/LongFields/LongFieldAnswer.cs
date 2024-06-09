namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongFieldAnswer : IFieldAnswer
    {
        private readonly LongField textField;
        private readonly long? answer;

        public LongFieldAnswer(LongField textField, long? answer)
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
