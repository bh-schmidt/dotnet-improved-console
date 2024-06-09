namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionAnswer : IFieldAnswer
    {
        private readonly TextOption textOption;
        private readonly string? answer;

        public TextOptionAnswer(
            TextOption textOption,
            string? answer)
        {
            this.textOption = textOption;
            this.answer = answer;
        }

        public IField Field => textOption;

        public string GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textOption.Title);
            var answer = this.answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
