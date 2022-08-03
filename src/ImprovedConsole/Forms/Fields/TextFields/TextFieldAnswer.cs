namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldAnswer : IFieldAnswer
    {
        private readonly TextField textField;
        private readonly string? answer;

        public TextFieldAnswer(TextField textField, string? answer)
        {
            this.textField = textField;
            this.answer = answer;
        }

        public IField Field => textField;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textField.Title);
            var answer = this.answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
