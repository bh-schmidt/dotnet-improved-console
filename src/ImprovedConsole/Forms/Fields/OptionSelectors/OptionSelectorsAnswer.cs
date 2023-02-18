namespace ImprovedConsole.Forms.Fields.OptionSelectors
{
    public class OptionSelectorsAnswer : IFieldAnswer
    {
        private readonly OptionSelector optionSelector;
        private readonly string? answer;

        public OptionSelectorsAnswer(
            OptionSelector optionSelector,
            string? answer)
        {
            this.optionSelector = optionSelector;
            this.answer = answer;
        }

        public IField Field => optionSelector;

        public string GetFormattedAnswer(FormOptions options)
        {
            var title = Message.RemoveColors(optionSelector.Title);
            var answer = this.answer ?? "Not Answered";

            return
$@"{{color:{options.TitleColor}}}{title}
 {{color:{options.AnswerColor}}}- {answer}";
        }
    }
}
