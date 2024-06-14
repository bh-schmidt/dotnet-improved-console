namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class PossibilityItem(string possibility)
    {
        public string Value { get; } = possibility;
        public bool Checked { get; set; }
        internal int Position { get; set; }
    }
}
