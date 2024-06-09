namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldAnswer : IFieldAnswer
    {
        private readonly TextField textField;

        public TextFieldAnswer(TextField textField, string? answer)
        {
            this.textField = textField;
            Answer = answer;
        }

        public IField Field => textField;
        public string? Answer { get; set; }

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textField.Title);
            var answer = Answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
