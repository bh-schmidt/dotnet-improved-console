namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class OptionItem<TFieldType>(TFieldType value)
    {
        public TFieldType Value { get; } = value;
        public bool Checked { get; set; }
        internal int Position { get; set; }
    }
}
