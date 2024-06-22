using ImprovedConsole.Forms.Fields.SingleSelects;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldAnswer<TFieldType>(
        TextField<TFieldType> textField,
        string title,
        TFieldType? answer) : IFieldAnswer
    {
        private readonly TextField<TFieldType> textField = textField;

        public IField Field => textField;
        public TFieldType? Answer { get; set; } = answer;

        public bool Equals(IFieldAnswer? other)
        {
            if (other is not TextFieldAnswer<TFieldType> text)
                return false;

            return Equals(Answer, text.Answer);
        }

        StringBuilder IFieldAnswer.GetFormattedAnswer(int leftSpacing, FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Answer is null ?
                "N/A" :
                textField.ConvertToStringDelegate(Answer);

            return new StringBuilder()
                .AppendLine($"{{color:{options.TitleColor}}}{formattedTitle}")
                .Append(' ', leftSpacing)
                .Append($"{{color:{options.AnswerColor}}}{answer}");
        }
    }
}
