namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldAnswer(TextField textField, string? answer) : IFieldAnswer
    {
        private readonly TextField textField = textField;

        public IField Field => textField;
        public string? Answer { get; set; } = answer;

        string IFieldAnswer.GetFormattedAnswer(FormOptions options)
        {
            string? title = Message.RemoveColors(textField.Title);
            string answer = Answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
