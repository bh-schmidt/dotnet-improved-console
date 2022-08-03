namespace ImprovedConsole.Forms.Fields
{
    public interface IFieldAnswer
    {
        public IField Field { get; }
        public string GetFormattedAnswer(FormOptions options);
    }
}
