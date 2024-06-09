namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionAnswer : IFieldAnswer
    {
        private readonly TextOption textOption;

        public TextOptionAnswer(
            TextOption textOption,
            string? answer)
        {
            this.textOption = textOption;
            this.Answer = answer;
        }

        public IField Field => textOption;
        public string? Answer { get; set; }

        public string GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(textOption.Title);
            var answer = this.Answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
