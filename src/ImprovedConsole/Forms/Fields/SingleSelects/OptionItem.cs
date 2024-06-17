namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class OptionItem<TFieldType>(TFieldType value)
    {
        public TFieldType Value { get; } = value;
        public bool Checked { get; set; }
    }
}
