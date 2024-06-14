namespace ImprovedConsole.Forms.Fields.LongFields
{
    public class LongFieldAnswer(LongField textField, long? answer) : IFieldAnswer
    {
        private readonly LongField textField = textField;

        public IField Field => textField;
        public long? Answer { get; } = answer;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? title = Message.RemoveColors(textField.Title);
            string answer = Answer?.ToString() ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
