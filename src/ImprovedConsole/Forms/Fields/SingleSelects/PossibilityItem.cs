namespace ImprovedConsole.Forms.Fields.SingleSelects
{
    public class PossibilityItem(string possibility)
    {
        public string Value { get; } = possibility;
        public bool Checked { get; set; }
        internal int Position { get; set; }
    }
}
