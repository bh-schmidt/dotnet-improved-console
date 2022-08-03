namespace ImprovedConsole.Forms
{
    public class FormOptions
    {
        public bool ShowConfirmationForms { get; set; } = true;
        public ConsoleColor TitleColor { get; set; } = ConsoleWriter.GetForegroundColor();
        public ConsoleColor AnswerColor { get; set; } = ConsoleColor.DarkGreen;
    }
}
