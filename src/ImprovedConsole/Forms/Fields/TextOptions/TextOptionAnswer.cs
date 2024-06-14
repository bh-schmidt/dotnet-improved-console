namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionAnswer(
        TextOption textOption,
        string? answer) : IFieldAnswer
    {
        private readonly TextOption textOption = textOption;

        public IField Field => textOption;
        public string? Answer { get; set; } = answer;

        public string GetFormattedAnswer(FormOptions options)
        {
            string? title = Message.RemoveColors(textOption.Title);
            string answer = this.Answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
