namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongFieldAnswer : IFieldAnswer
    {
        private readonly LongField textField;
        public LongFieldAnswer(LongField textField, long? answer)
        {
            this.textField = textField;
            Answer = answer;
        }

        public IField Field => textField;
        public long? Answer { get; }

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textField.Title);
            var answer = Answer?.ToString() ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
