namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionAnswer<TFieldType>(
        TextOption<TFieldType> textOption,
        string title,
        TFieldType? answer) : IFieldAnswer
    {
        public IField Field => textOption;
        public TFieldType? Answer { get; set; } = answer;

        public string GetFormattedAnswer(FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Answer?.ToString() ?? "N/A";

            return
$@"{{color:{options.TitleColor}}}{formattedTitle}
   {{color:{options.AnswerColor}}}{answer}";
        }
    }
}
