using ImprovedConsole.Forms.Fields.MultiSelects;
using System.Text;

namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class SingleSelectAnswer<TFieldType>(SingleSelect<TFieldType> singleSelect, string title, TFieldType? selection) : IFieldAnswer
    {
        public IField Field => singleSelect;
        public TFieldType? Selection { get; set; } = selection;

        public bool Equals(IFieldAnswer? other)
        {
            if (other is not SingleSelectAnswer<TFieldType> select)
                return false;

            return Equals(Selection, select.Selection);
        }

        StringBuilder IFieldAnswer.GetFormattedAnswer(int leftSpacing, FormOptions options)
        {
            string? formattedTitle = Message.RemoveColors(title);
            string answer = Selection is null ?
                "N/A" :
                singleSelect.ConvertToStringDelegate(Selection);

            return new StringBuilder()
                .AppendLine($"{{color:{options.TitleColor}}}{formattedTitle}")
                .Append(' ', leftSpacing)
                .Append($"{{color:{options.AnswerColor}}}{answer}");
        }
    }
}
