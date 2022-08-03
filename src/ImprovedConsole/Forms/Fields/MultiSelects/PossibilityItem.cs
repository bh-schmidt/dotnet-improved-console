namespace ImprovedConsole.Forms.Fields.MultiSelects
{
    public class PossibilityItem
    {
        public PossibilityItem(string possibility)
        {
            Value = possibility;
        }

        public string Value { get; }
        public bool Checked { get; set; }
        internal int Position { get; set; }
    }
}
