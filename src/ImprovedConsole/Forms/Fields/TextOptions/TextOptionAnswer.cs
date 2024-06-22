using ImprovedConsole.Forms.Fields.TextFields;
using System.Text;

namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionAnswer<TFieldType>(
        TextOption<TFieldType> textOption,
        string title,
        TFieldType? answer) : IFieldAnswer
    {
        public IField Field => textOption;
        public TFieldType? Answer { get; set; } = answer;

        public bool Equals(IFieldAnswer? other)
        {
            if (other is not TextOptionAnswer<TFieldType> text)
                return false;

            return Equals(Answer, text.Answer);
        }

        public StringBuilder GetFormattedAnswer(int leftSpacing, FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Answer is null?
                "N/A" :
                textOption.ConvertToStringDelegate(Answer);

            return new StringBuilder()
                .AppendLine($"{{color:{options.TitleColor}}}{formattedTitle}")
                .Append(' ', leftSpacing)
                .Append($"{{color:{options.AnswerColor}}}{answer}");
        }
    }
}
