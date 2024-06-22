namespace ImprovedConsole.Forms
{
    public class FormOptions
    {
        public ConfirmationType ConfirmationType { get; set; } = ConfirmationType.SingleSelect;
        public bool PrintAnswersWhenFinish { get; set; } = true;
        public ConsoleColor TitleColor { get; set; } = ConsoleWriter.GetForegroundColor();
        public ConsoleColor AnswerColor { get; set; } = ConsoleColor.DarkGreen;
    }

    public enum ConfirmationType
    {
        None = 0,
        TextOption = 1,
        SingleSelect = 2
    }
}
